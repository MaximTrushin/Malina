using Malina.DOM;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System;
using System.Linq;

namespace Malina.Compiler.Steps
{
    class CompilingToFileVisitor : MalinaDepthFirstVisitor
    {
        private CompilerContext _context;
        private Document _currentDocument;
        private XmlWriter _xmlTextWriter;
        private Stack<AliasContext> _aliasDefinitionContext;

        public Stack<AliasContext> AliasContext
        {
            get
            {
                if (_aliasDefinitionContext == null)
                {
                    _aliasDefinitionContext = new Stack<AliasContext>();
                    _aliasDefinitionContext.Push(null);
                }
                return _aliasDefinitionContext;
            }

            set
            {
                _aliasDefinitionContext = value;
            }
        }

        public CompilingToFileVisitor(CompilerContext context)
        {
            _context = context;
        }

        public override void OnDocument(Document node)
        {
            _currentDocument = node;
            var fileName = _context.Parameters.OutputDirectory + node.Name + ".xml";
            using (_xmlTextWriter = XmlWriter.Create(new XmlTextWriter(fileName, Encoding.UTF8) { Formatting = System.Xml.Formatting.Indented, Namespaces = true }))
            {
                _xmlTextWriter.WriteStartDocument();

                base.OnDocument(node);
                _xmlTextWriter.WriteEndDocument();

            }
            _currentDocument = null;
        }

        public override void OnAttribute(DOM.Attribute node)
        {
            string prefix, ns;
            var aliasContext = AliasContext.Peek();
            _context.NamespaceResolver.GetPrefixAndNs(node, _currentDocument, aliasContext == null ? null : aliasContext.AliasDefinition, out prefix, out ns);

            _xmlTextWriter.WriteAttributeString(prefix, node.Name, ns, node.Value);
            base.OnAttribute(node);
        }
        public override void OnElement(Element node)
        {
            string prefix, ns;
            AliasContext aliasContext = AliasContext.Peek();
            _context.NamespaceResolver.GetPrefixAndNs(node, _currentDocument, aliasContext == null? null: aliasContext.AliasDefinition, out prefix, out ns);

            _xmlTextWriter.WriteStartElement(prefix, node.Name, ns);

            if (node.Parent is Document)
            {
                WritePendingNamespaceDeclarations(ns);
            }

            if (node.Value != null) {
                _xmlTextWriter.WriteString(node.Value);
            }
            else if (node.ObjectValue is Parameter)
            {
                ResolveValueParameter(node.ObjectValue as Parameter, aliasContext);
            }
            base.OnElement(node);

            _xmlTextWriter.WriteEndElement();
        }

        private void ResolveValueParameter(Parameter parameter, AliasContext aliasContext)
        {
            //throw new NotImplementedException();
            var alias = aliasContext.Alias;

            var argument = alias.Arguments.FirstOrDefault(a => a.Name == parameter.Name);

            if (argument != null)
            {
                _xmlTextWriter.WriteString(argument.Value);
            }
            else
            {
                if (parameter.Value != null) _xmlTextWriter.WriteString(parameter.Value);
            }


        }

        private void WritePendingNamespaceDeclarations(string uri)
        {
            NamespaceResolver.NsInfo nsInfo = _context.NamespaceResolver.GetNsInfo(_currentDocument);
            if (nsInfo == null) return;

            foreach (var ns in nsInfo.Namespaces)
            {
                if (ns.Value == uri) continue;
                _xmlTextWriter.WriteAttributeString("xmlns", ns.Name, null, ns.Value);

            }
        }

        public override void OnAlias(Alias node)
        {
            var aliasName = node.Name;
            base.OnAlias(node);
            var aliasDef = _context.NamespaceResolver.GetAliasDefinition(node.Name);

            if (aliasDef == null) return;

            AliasContext.Push(new AliasContext() { AliasDefinition = aliasDef, Alias = node, AliasNsInfo = GetContextNsInfo() });
            base.OnAliasDefinition(aliasDef);
            AliasContext.Pop();
        }

        private NamespaceResolver.NsInfo GetContextNsInfo()
        {
            if (AliasContext.Peek() == null)
            {
                return _context.NamespaceResolver.GetNsInfo(_currentDocument);
            }

            return _context.NamespaceResolver.GetNsInfo(AliasContext.Peek().AliasDefinition);
        }

        public override void OnAliasDefinition(AliasDefinition node)
        {
             //Doing nothing for Alias Definition        
        }
    }
}
