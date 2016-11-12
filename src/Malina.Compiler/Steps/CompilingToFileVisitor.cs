using Malina.DOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Malina.Compiler.Steps
{
    class CompilingToFileVisitor : MalinaDepthFirstVisitor
    {
        private CompilerContext _context;
        private Document _currentDocument;
        private XmlTextWriter _xmlTextWriter;

        public CompilingToFileVisitor(CompilerContext context)
        {
            _context = context;
        }

        public override void OnDocument(Document node)
        {
            _currentDocument = node;
            var fileName = _context.Parameters.OutputDirectory + node.Name + ".xml";
            using (_xmlTextWriter = new XmlTextWriter(fileName, Encoding.UTF8) { Formatting = System.Xml.Formatting.Indented, Namespaces = true })
            {
                _xmlTextWriter.WriteStartDocument();
                base.OnDocument(node);
                _xmlTextWriter.WriteEndDocument();

            }
            _currentDocument = null;
        }

        public override void OnAttribute(DOM.Attribute node)
        {
            _xmlTextWriter.WriteAttributeString(node.NsPrefix, node.Name, null, node.Value);
            base.OnAttribute(node);
        }
        public override void OnElement(Element node)
        {
            if (node.Value != null)
            {
                if (node.Value is string) { _xmlTextWriter.WriteElementString(node.NsPrefix, node.Name, null, node.Value); }
            }
            else
            {
                _xmlTextWriter.WriteStartElement(node.NsPrefix, node.Name, null);
            } 
            base.OnElement(node);
        }

        public override void OnAlias(Alias node)
        {
            var aliasName = node.Name;
            base.OnAlias(node);
            var aliasDef = _context.AliasDefinitions[node.Name];
            base.OnAliasDefinition(aliasDef);
        }

        public override void OnAliasDefinition(AliasDefinition node)
        {            
        }
    }
}
