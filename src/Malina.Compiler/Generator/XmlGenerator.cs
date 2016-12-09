using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Antlr4.Runtime;
using Malina.Compiler.Generator;
using Malina.Compiler.Steps;
using Malina.DOM;
using Malina.Parser;
using ValueType = Malina.DOM.ValueType;

namespace Malina.Compiler
{
    public class XmlGenerator : MalinaDepthFirstVisitor
    {
        private Document _currentDocument;
        private XmlWriter _xmlTextWriter;
        private readonly Func<string, XmlWriter> _writerDelegate;
        private readonly CompilerContext _context;
        private bool _rootElementAdded;
        private Stack<AliasContext> _aliasContext;


        public List<LexicalInfo> LocationMap { get; set; }

        public Stack<AliasContext> AliasContext
        {
            get
            {
                if (_aliasContext != null) return _aliasContext;

                _aliasContext = new Stack<AliasContext>();
                _aliasContext.Push(null);

                return _aliasContext;
            }

            set
            {
                _aliasContext = value;
            }
        }

        /// <summary>
        /// This constructor should be used if output depends on the name of the document.
        /// </summary>
        /// <param name="writerDelegate">Delegate will be called for each Document. The name of the document will be sent in the string argument.</param>
        /// <param name="context"></param>
        public XmlGenerator(Func<string, XmlWriter> writerDelegate, CompilerContext context)
        {
            _writerDelegate = writerDelegate;
            _context = context;
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
            if (node.Value != null)
            {
                _xmlTextWriter.WriteString(ResolveNodeValue((DOM.Antlr.IValueNode)node));
            }
            else if (node.ObjectValue is Parameter)
            {
                _xmlTextWriter.WriteString(ResolveValueParameter((Parameter)node.ObjectValue));
            }
            else if (node.ObjectValue is Alias)
            {
                _xmlTextWriter.WriteString(ResolveValueAlias((Alias)node.ObjectValue));
            }

            Visit(node.Entities);

            //End Element
            _xmlTextWriter.WriteEndElement();
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
        
        public override void OnAlias(Alias alias)
        {
            var aliasDef = _context.NamespaceResolver.GetAliasDefinition(alias.Name);

            AliasContext.Push(new AliasContext() { AliasDefinition = aliasDef, Alias = alias, AliasNsInfo = GetContextNsInfo() });
            Visit(aliasDef.Entities);
            AliasContext.Pop();
        }

        public override void OnParameter(Parameter parameter)
        {
            var aliasContext = AliasContext.Peek();
            var argument = aliasContext.Alias.Arguments.FirstOrDefault(a => a.Name == parameter.Name);

            Visit(argument != null ? argument.Entities : parameter.Entities);
        }


        public override void OnAliasDefinition(AliasDefinition node)
        {
            //Doing nothing for Alias Definition        
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
                if (entity is Alias)
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
                    return ResolveValueAlias((Alias)node.ObjectValue);
                }
            }
            else return node.Value;

            return null;
        }

        private string ResolveNodeValue(DOM.Antlr.IValueNode node)
        {
            if (((IValueNode)node).ValueType != ValueType.SingleQuotedString)
                return ((IValueNode)node).Value;

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

                sb.Append(token.Type == MalinaLexer.SQS_EOL ? ResolveSqsEol(token, node.ValueIndent) : token.Text);
            }
            return sb.ToString();

        }

        private string ResolveValueParameter(Parameter parameter)
        {
            var aliasContext = AliasContext.Peek();
            var argument = aliasContext.Alias.Arguments.FirstOrDefault(a => a.Name == parameter.Name);
            if (argument != null)
            {
                if (argument.ObjectValue != null)
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

        private static string ResolveSqsEol(IToken token, int valueIndent)
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

        private NsInfo GetContextNsInfo()
        {
            if (AliasContext.Peek() == null)
            {
                return _context.NamespaceResolver.GetNsInfo(_currentDocument);
            }

            return _context.NamespaceResolver.GetNsInfo(AliasContext.Peek().AliasDefinition);
        }
    }
}
