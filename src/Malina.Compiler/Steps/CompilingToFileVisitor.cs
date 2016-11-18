using Malina.DOM;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Malina.Compiler.Steps
{
    class CompilingToFileVisitor : MalinaDepthFirstVisitor
    {
        private CompilerContext _context;
        private Document _currentDocument;
        private XmlWriter _xmlTextWriter;
        private Stack<AliasDefinition> _aliasDefinitionContext;

        public Stack<AliasDefinition> AliasDefinitionContext
        {
            get
            {
                if (_aliasDefinitionContext == null)
                {
                    _aliasDefinitionContext = new Stack<AliasDefinition>();
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
            _context.NamespaceResolver.GetPrefixAndNs(node, _currentDocument, AliasDefinitionContext.Peek(), out prefix, out ns);

            _xmlTextWriter.WriteAttributeString(prefix, node.Name, ns, node.Value);
            base.OnAttribute(node);
        }
        public override void OnElement(Element node)
        {
            string prefix, ns;
            _context.NamespaceResolver.GetPrefixAndNs(node, _currentDocument, AliasDefinitionContext.Peek(), out prefix, out ns);

            _xmlTextWriter.WriteStartElement(prefix, node.Name, ns);

            if (node.Value is string) { _xmlTextWriter.WriteString(node.Value); }
            base.OnElement(node);

            _xmlTextWriter.WriteEndElement();
        }

        public override void OnAlias(Alias node)
        {
            var aliasName = node.Name;
            base.OnAlias(node);
            AliasDefinition aliasDef = null;
            _context.AliasDefinitions.TryGetValue(node.Name, out aliasDef);

            if (aliasDef == null) return;

            AliasDefinitionContext.Push(aliasDef);
            base.OnAliasDefinition(aliasDef);
            AliasDefinitionContext.Pop();
        }

        public override void OnAliasDefinition(AliasDefinition node)
        {
                        
        }
    }
}
