using System;
using Malina.DOM;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Schema;
using Antlr4.Runtime;
using Malina.Parser;
using ValueType = Malina.DOM.ValueType;

namespace Malina.Compiler.Steps
{
    class CompilingToFileVisitor : MalinaDepthFirstVisitor
    {
        private readonly CompilerContext _context;
        private Document _currentDocument;
        private XmlWriter _xmlTextWriter;
        private Stack<AliasContext> _aliasContext;
        private bool _rootElementAdded;
        private Stack<int> _indicesStack;
        private int _validationIndex;

        private List<LexicalInfo> LocationMap { get; set; } = new List<LexicalInfo>();

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

            //Generate XML file
            using (_xmlTextWriter = XmlWriter.Create(
                new XmlTextWriter(fileName, Encoding.UTF8) { Formatting = Formatting.Indented, Namespaces = true },
                new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Document })
            )
            {
                _xmlTextWriter.WriteStartDocument();
                _rootElementAdded = false;
                LocationMap = new List<LexicalInfo>();
                base.OnDocument(node);
                _xmlTextWriter.WriteEndDocument();

            }

            //Validate XML file
            ValidateGeneratedFile(fileName);

            _currentDocument = null;
        }

        private void ValidateGeneratedFile(string fileName)
        {
            try
            {
                var settings = new XmlReaderSettings
                {
                    ConformanceLevel = ConformanceLevel.Document,
                    ValidationFlags = XmlSchemaValidationFlags.AllowXmlAttributes |
                                      XmlSchemaValidationFlags.ReportValidationWarnings,
                    Schemas = _context.Parameters.XmlSchemaSet
                };

                if (_context.Parameters.XmlSchemaSet.Count > 0) settings.ValidationType = ValidationType.Schema;

                    _validationIndex = 0;
                _indicesStack = new Stack<int>();
                _indicesStack.Push(0);
                settings.ValidationEventHandler += ValidationEventHandler;


                using (var textReader = new XmlTextReader(fileName))
                {
                    using (var reader = XmlReader.Create(textReader, settings))
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                if (!reader.IsEmptyElement)
                                    _indicesStack.Push(_validationIndex); //This stack is used to store index of the current parent element.

                                _validationIndex++;
                                _validationIndex += GetAttributesCount(reader);
                            }

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                _indicesStack.Pop();
                            }
                        }
                    }
                }
            }
            catch (XmlSchemaValidationException ex)
            {
                _context.AddError(CompilerErrorFactory.XmlSchemaValidationError(ex));
            }
        }

        private int GetAttributesCount(XmlReader reader)
        {
            if (!reader.MoveToFirstAttribute())
                return 0;
            var count = 0;
            do
            {
                if (reader.NamespaceURI != "http://www.w3.org/2000/xmlns/")
                    count++;
            } while (reader.MoveToNextAttribute());
            return count;
        }

        private void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            var index = ((XmlReader) sender).NodeType == XmlNodeType.EndElement ? _indicesStack.Peek() : _validationIndex;

            var location = LocationMap[index];
            _context.Errors.Add(CompilerErrorFactory.XmlSchemaValidationError(e.Exception, location));
        }

        public override void OnAttribute(DOM.Attribute node)
        {
            string prefix, ns;
            
            _context.NamespaceResolver.GetPrefixAndNs(node, _currentDocument, 
                () => { var aliasContext = AliasContext.Peek(); return aliasContext?.AliasDefinition; }, 
                out prefix, out ns); 
            string value = ResolveAttributeValue(node);
            _xmlTextWriter.WriteAttributeString(prefix, node.Name, ns, value);
            LocationMap.Add(new LexicalInfo(_currentDocument.Module.FileName, node.start.Line, node.start.Column, node.start.Index));

        }

        private string ResolveAttributeValue(DOM.Attribute node)
        {
            if (node.ObjectValue != null)
            {
                var value = node.ObjectValue as Parameter;
                if (value != null)
                {
                    return ResolveValueParameter(value);
                }
                else if (node.ObjectValue is Alias)
                {
                    return ResolveValueAlias((Alias) node.ObjectValue);
                }
            }
            else return node.Value;

            return null;
        }

        private string ResolveValueAlias(Alias alias)
        {
            var aliasDef = _context.NamespaceResolver.GetAliasDefinition(alias.Name);

            if (aliasDef.ObjectValue == null) return aliasDef.Value;

            return ResolveObjectValue(aliasDef.ObjectValue);

        }

        private string ResolveObjectValue(object objectValue)
        {
            var value = objectValue as Parameter;
            if (value != null)
            {
                return ResolveValueParameter(value);
            }

            var alias = objectValue as Alias;
            if (alias != null)
            {
                return ResolveValueAlias(alias);
            }
            return null;
        }

        private string ResolveValueParameter(Parameter parameter)
        {
            var aliasContext = AliasContext.Peek();
            var argument = aliasContext.Alias.Arguments.FirstOrDefault(a => a.Name == parameter.Name);
            if (argument != null)
            {
                if(argument.ObjectValue != null)
                {
                    return ResolveObjectValue(argument.ObjectValue);
                }
                return argument.Value;
            } 

            //if argument is not found lookup default value in the Alias Definition
            var paramDef = (aliasContext.AliasDefinition as DOM.Antlr.AliasDefinition)?.Parameters.First(p => p.Name == parameter.Name);

            //If parameteres default value is Parameter or Alias then resolve it
            if (paramDef?.ObjectValue != null)
            {
                return ResolveObjectValue(paramDef.ObjectValue);

            }

            return paramDef?.Value;

        }

        public override void OnElement(Element node)
        {
            //Getting namespace and prefix
            string prefix, ns;
            _context.NamespaceResolver.GetPrefixAndNs(node, _currentDocument,
                () => { var aliasContext = AliasContext.Peek(); return aliasContext?.AliasDefinition; },
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


            //Write attributes            
            ResolveAttributes(node.Attributes, node.Entities);

            //Write element's value
            if (node.Value != null) {
                _xmlTextWriter.WriteString(ResolveNodeValue((DOM.Antlr.IValueNode) node));
            }
            else if (node.ObjectValue is Parameter)
            {
                _xmlTextWriter.WriteString(ResolveValueParameter((Parameter) node.ObjectValue));
            }
            else if (node.ObjectValue is Alias)
            {
                _xmlTextWriter.WriteString(ResolveValueAlias((Alias) node.ObjectValue));
            }

            Visit(node.Entities);

            //End Element
            _xmlTextWriter.WriteEndElement();
        }

        private string ResolveNodeValue(DOM.Antlr.IValueNode node)
        {
            if (((IValueNode) node).ValueType != ValueType.SingleQuotedString)
                return ((IValueNode) node).Value;

            var sb = new StringBuilder();
            foreach (var item in node.InterpolationItems)
            {
                var alias = item as Alias;
                if (alias != null)
                {
                    sb.Append(ResolveValueAlias(alias));
                    continue;
                }
                var token = item as CommonToken;
                if (token == null) continue;

                if (token.Type == MalinaLexer.SQS_EOL)
                {
                    sb.Append(ResolveSqsEol(token, node.ValueIndent));
                }
                else
                    sb.Append(token.Text);
            }
            return sb.ToString();

        }

        private string ResolveSqsEol(CommonToken token, int valueIndent)
        {
            var sb = new StringBuilder();
            var value = token.Text;
            var lines = value.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var first = true;
            foreach (var item in lines)
            {
                //Skip first one
                if (first)
                {
                    first = false;
                    continue;
                }

                //Removing indents
                sb.AppendLine();
                if (item.Length > valueIndent)
                    sb.Append(item.Substring(valueIndent));
            }

            return sb.ToString();
        }

        private void ResolveAttributes(IEnumerable<DOM.Attribute> attributes, IEnumerable<Entity> entities)
        {
            Visit(attributes);
            ResolveAttributes(entities);
        }

        /// <summary>
        /// Go through all entities and resolve attributes for the current node.
        /// </summary>
        /// <param name="entities">List of entities. Looking for alias or parameter because they potentially can hold the attributes.</param>
        private void ResolveAttributes(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                if(entity is Alias)
                {
                    ResolveAttributesInAlias(entity as Alias);
                }
                else if (entity is Parameter)
                {
                    ResolveAttributesInParameter(entity as Parameter);
                }
            }
        }

        private void ResolveAttributesInParameter(Parameter parameter)
        {
            var aliasContext = AliasContext.Peek();
            var argument = aliasContext.Alias.Arguments.FirstOrDefault(a => a.Name == parameter.Name);
            if (argument != null)
            {
                //Resolve using argument's block
                ResolveAttributes(argument.Attributes, argument.Entities);
            }
            else
            {
                //Resolve using parameter's default block
                ResolveAttributes(parameter.Attributes, parameter.Entities);
            }
        }

        private void ResolveAttributesInAlias(Alias alias)
        {
            var aliasDef = _context.NamespaceResolver.GetAliasDefinition(alias.Name);
            AliasContext.Push(new AliasContext() { AliasDefinition = aliasDef, Alias = alias, AliasNsInfo = GetContextNsInfo() });
            ResolveAttributes(aliasDef.Attributes, aliasDef.Entities);
            AliasContext.Pop();
        }

        public override void OnParameter(Parameter parameter)
        {
            var aliasContext = AliasContext.Peek();
            var argument = aliasContext.Alias.Arguments.FirstOrDefault(a => a.Name == parameter.Name);

            Visit(argument != null ? argument.Entities : parameter.Entities);
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

        public override void OnAlias(Alias alias)
        {
            var aliasDef = _context.NamespaceResolver.GetAliasDefinition(alias.Name);

            AliasContext.Push(new AliasContext() { AliasDefinition = aliasDef, Alias = alias, AliasNsInfo = GetContextNsInfo() });
            Visit(aliasDef.Entities);
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
