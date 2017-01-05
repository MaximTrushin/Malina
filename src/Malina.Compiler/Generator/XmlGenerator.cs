using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Antlr4.Runtime;
using Malina.DOM;
using ValueType = Malina.DOM.ValueType;

namespace Malina.Compiler.Generator
{
    public class XmlGenerator : AliasResolvingVisitor
    {
        private XmlWriter _xmlTextWriter;
        private readonly Func<string, XmlWriter> _writerDelegate;
        private bool _rootElementAdded;

        public List<LexicalInfo> LocationMap { get; set; }

        /// <summary>
        /// This constructor should be used if output depends on the name of the document.
        /// </summary>
        /// <param name="writerDelegate">Delegate will be called for each Document. The name of the document will be sent in the string argument.</param>
        /// <param name="context"></param>
        public XmlGenerator(Func<string, XmlWriter> writerDelegate, CompilerContext context):base(context)
        {
            _writerDelegate = writerDelegate;
        }

        public override void OnDocument(Document node)
        {
            _currentDocument = node;

            //Generate XML file
            using (_xmlTextWriter = _writerDelegate(node.Name))
            {
                _xmlTextWriter.WriteStartDocument();
                _rootElementAdded = false;
                LocationMap = new List<LexicalInfo>();
                base.OnDocument(node);
                _xmlTextWriter.WriteEndDocument();

            }

            //Validate XML file
            var validator = new SourceMappedXmlValidator(LocationMap, _context.Parameters.XmlSchemaSet);
            validator.ValidationErrorEvent += error => _context.Errors.Add(error);
            
            var fileName = Path.Combine(_context.Parameters.OutputDirectory, node.Name + ".xml");
            validator.ValidateGeneratedFile(fileName);

            _currentDocument = null;
        }


        public override void OnElement(Element node)
        {
            //Getting namespace and prefix
            string prefix, ns;
            _context.NamespaceResolver.GetPrefixAndNs(node, _currentDocument,
                () => { var aliasContext = AliasContext.Peek(); return aliasContext?.AliasDefinition; },
                () => ScopeContext.Peek(),
                out prefix, out ns);

            //Starting Element
            _xmlTextWriter.WriteStartElement(prefix, node.Name, ns);
            LocationMap.Add(new LexicalInfo(_currentDocument.Module.FileName, node.start.Line, node.start.Column, node.start.Index));

            //Write all namespace declarations in the root element
            if (!_rootElementAdded)
            {
                WritePendingNamespaceDeclarations(ns);
                _rootElementAdded = true;
            }

            if (!ResolveValue(node))
                base.OnElement(node);

            //End Element
            _xmlTextWriter.WriteEndElement();
        }

        public override void OnValue(string value, ValueType type)
        {
            _xmlTextWriter.WriteString(value);
        }

        protected override void ResolveSqsEscape(CommonToken token, StringBuilder sb)
        {
            char c = ResolveSqsEscapeChar(token);
            if (IsLegalXmlChar(c))
            {
                sb.Append(c);
            }
        }



        /// <summary>
        /// Whether a given character is allowed by XML 1.0.
        /// </summary>
        public static bool IsLegalXmlChar(int character)
        {
            return
            (
                 character == 0x9 /* == '\t' == 9   */          ||
                 character == 0xA /* == '\n' == 10  */          ||
                 character == 0xD /* == '\r' == 13  */          ||
                (character >= 0x20 && character <= 0xD7FF) ||
                (character >= 0xE000 && character <= 0xFFFD) ||
                (character >= 0x10000 && character <= 0x10FFFF)
            );
        }

        public override void OnAttribute(DOM.Attribute node)
        {
            string prefix, ns;

            _context.NamespaceResolver.GetPrefixAndNs(node, _currentDocument,
                () => { var aliasContext = AliasContext.Peek(); return aliasContext?.AliasDefinition; },
                () => ScopeContext.Peek(),
                out prefix, out ns);
            
            _xmlTextWriter.WriteStartAttribute(prefix, node.Name, ns);
            ResolveValue(node);
            _xmlTextWriter.WriteEndAttribute();

            LocationMap.Add(new LexicalInfo(_currentDocument.Module.FileName, node.start.Line, node.start.Column, node.start.Index));
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


        private string ResolveAttributeValue(DOM.Attribute node)
        {
            if (node.ObjectValue != null)
            {
                var value = node.ObjectValue as Parameter;
                ValueType valueType;
                if (value != null)
                {
                    return ResolveValueParameter(value, out valueType);
                }
                else if (node.ObjectValue is Alias)
                {
                    return ResolveValueAlias((Alias)node.ObjectValue, out valueType);
                }
            }
            else return node.Value;

            return null;
        }





    }
}
