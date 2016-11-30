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
        private Stack<AliasContext> _aliasContext;
        private bool _rootElementAdded = false;

        public Stack<AliasContext> AliasContext
        {
            get
            {
                if (_aliasContext == null)
                {
                    _aliasContext = new Stack<AliasContext>();
                    _aliasContext.Push(null);
                }
                return _aliasContext;
            }

            set
            {
                _aliasContext = value;
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
                _rootElementAdded = false;
                base.OnDocument(node);
                _xmlTextWriter.WriteEndDocument();

            }
            _currentDocument = null;
        }

        public override void OnAttribute(DOM.Attribute node)
        {
            string prefix, ns;

            
            _context.NamespaceResolver.GetPrefixAndNs(node, _currentDocument, 
                () => { var aliasContext = AliasContext.Peek(); return aliasContext == null ? null : aliasContext.AliasDefinition; }, 
                out prefix, out ns);
            string value = ResolveAttributeValue(node);
            _xmlTextWriter.WriteAttributeString(prefix, node.Name, ns, value);
            base.OnAttribute(node);
        }

        private string ResolveAttributeValue(DOM.Attribute node)
        {
            if (node.ObjectValue != null)
            {
                if (node.ObjectValue is Parameter)
                {
                    return ResolveValueParameter(node.ObjectValue as Parameter);
                }
                else if (node.ObjectValue is Alias)
                {
                    return ResolveValueAlias(node.ObjectValue as Alias);
                }                
            }
            else return node.Value;

            return null;
        }

        private string ResolveValueAlias(Alias alias)
        {
            throw new NotImplementedException();
        }

        private string ResolveValueParameter(Parameter parameter)
        {
            var aliasContext = AliasContext.Peek();
            var argument = aliasContext.Alias.Arguments.FirstOrDefault(a => a.Name == parameter.Name);

            if (argument != null) return argument.Value;

            //if argument is not found lookup default value in the Alias Definition
            var paramDef = (aliasContext.AliasDefinition as DOM.Antlr.AliasDefinition).Parameters.First(p => p.Name == parameter.Name);

            //If parameteres default value is Parameter or Alias then resolve it
            if (paramDef.ObjectValue != null)
            {
                if (paramDef.ObjectValue is Parameter)
                {
                    return ResolveValueParameter(paramDef.ObjectValue as Parameter);
                }
                else if (paramDef.ObjectValue is Alias)
                {
                    return ResolveValueAlias(paramDef.ObjectValue as Alias);
                }
            }

            return paramDef.Value;

        }

        public override void OnElement(Element node)
        {
            //Getting namespace and prefix
            string prefix, ns;
            _context.NamespaceResolver.GetPrefixAndNs(node, _currentDocument,
                () => { var aliasContext = AliasContext.Peek(); return aliasContext == null ? null : aliasContext.AliasDefinition; },
                out prefix, out ns);

            //Starting Element
            _xmlTextWriter.WriteStartElement(prefix, node.Name, ns);

            //Write all namespace declarations in the root element
            if (!_rootElementAdded)
            {
                WritePendingNamespaceDeclarations(ns);
            }

            _rootElementAdded = true;

            //Write attributes            
            ResolveAttributes(node.Attributes, node.Entities);

            //Write element's value
            if (node.Value != null) {
                _xmlTextWriter.WriteString(node.Value);
            }
            else if (node.ObjectValue is Parameter)
            {
                ResolveValueParameter(node.ObjectValue as Parameter, AliasContext.Peek());
            }

            Visit(node.Entities);

            //End Element
            _xmlTextWriter.WriteEndElement();
        }

        private void ResolveAttributes(IEnumerable<DOM.Attribute> attributes, IEnumerable<Entity> entities)
        {
            Visit(attributes);
            ResolveAttributes(entities);
        }

        private void ResolveAttributes(IEnumerable<Entity> entities)
        {
            // todo: Create Attribute resolving visitor to write all attributes first
        }

        public override void OnParameter(Parameter node)
        {
            ResolveBlockParameter(node);
        }

        private void ResolveBlockParameter(Parameter parameter)
        {
            var aliasContext = AliasContext.Peek();
            var argument = aliasContext.Alias.Arguments.FirstOrDefault(a => a.Name == parameter.Name);

            if (argument != null)
            {
                //Resolve using argument's block
                Visit(argument.Entities);
            }
            else
            {
                //Resolve using parameter's default block
                Visit(parameter.Entities);
            }
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
            NsInfo nsInfo = _context.NamespaceResolver.GetNsInfo(_currentDocument);
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
            var aliasDef = _context.NamespaceResolver.GetAliasDefinition(node.Name);

            if (aliasDef == null) return;

            AliasContext.Push(new AliasContext() { AliasDefinition = aliasDef, Alias = node, AliasNsInfo = GetContextNsInfo() });
            base.OnAliasDefinition(aliasDef);
            AliasContext.Pop();
        }

        private NsInfo GetContextNsInfo()
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
