using System;
using System.Collections.Generic;
using System.Linq;
using Malina.DOM;
using Newtonsoft.Json;
using Attribute = Malina.DOM.Attribute;
using ValueType = Malina.DOM.ValueType;

namespace Malina.Compiler.Generator
{
    public class JsonGenerator : MalinaGenerator
    {
        private readonly Func<string, JsonWriter> _writerDelegate;

        private JsonWriter _jsonWriter;
        private Document _currentDocument;


        /// <summary>
        /// This constructor should be used if output depends on the name of the document.
        /// </summary>
        /// <param name="writerDelegate">Delegate will be called for the each Document. The name of the document will be sent in the string argument.</param>
        /// <param name="context"></param>
        public JsonGenerator(Func<string, JsonWriter> writerDelegate, CompilerContext context):base(context)
        {
            _writerDelegate = writerDelegate;
            _context = context;
        }

        public override void OnDocument(Document node)
        {
            _currentDocument = node;

            using (_jsonWriter = _writerDelegate(node.Name))
            {
                if (IsArrayNode(node))
                {
                    _jsonWriter.WriteStartArray();
                    base.OnDocument(node);
                    _jsonWriter.WriteEndArray();
                }
                else
                {
                    _jsonWriter.WriteStartObject();
                    base.OnDocument(node);
                    _jsonWriter.WriteEndObject();
                }
            }

            _currentDocument = null;
        }


        public virtual void OnValue(object value, ValueType type)
        {
            if (type == ValueType.Null)
            {
                _jsonWriter.WriteNull();
                return;
            }

            bool boolValue;
            if (type == ValueType.Boolean && bool.TryParse(value.ToString(), out boolValue))
            {
                _jsonWriter.WriteValue(boolValue);
            }
            else
            {
                decimal numberValue;
                if (type == ValueType.Number && decimal.TryParse(value.ToString(), out numberValue))
                    _jsonWriter.WriteValue(numberValue);
                else
                    _jsonWriter.WriteValue(value);
            }
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

        public override void OnAttribute(Attribute node)
        {
            _jsonWriter.WritePropertyName((node.NsPrefix != null ? node.NsPrefix + "." : "") + node.Name);
            ResolveValue(node);
        }

        public override void OnElement(Element node)
        {
            if (node.Name != string.Empty)
                _jsonWriter.WritePropertyName((node.NsPrefix != null ? node.NsPrefix + "." : "") + node.Name);

            if (ResolveValue(node)) return;



            if (IsArrayNode(node))
            {
                _jsonWriter.WriteStartArray();
                base.OnElement(node);
                _jsonWriter.WriteEndArray();
                return;
            }

            if (node.Attributes.Any() || node.Entities.Any() || node.ValueType == ValueType.Empty)
            {
                _jsonWriter.WriteStartObject();
                //Write attributes            
                ResolveAttributes(node.Attributes, node.Entities);
                Visit(node.Entities);
                _jsonWriter.WriteEndObject();
                return;
            }

            if (node.Name != null)
            {
                _jsonWriter.WriteStartObject();
                _jsonWriter.WriteEndObject();
                //return;
            }
        }

        private bool ResolveValue(IValueNode node)
        {
            //Write element's value
            object value = node.ObjectValue as Parameter;
            if (value != null)
            {
                OnValue(ResolveValueParameter((Parameter)value), node.ValueType);
                return true;
            }

            value = node.ObjectValue as Alias;
            if (value != null)
            {
                OnValue(ResolveValueAlias((Alias)value), node.ValueType);
                return true;
            }
            if (node.Value != null)
            {
                OnValue(ResolveNodeValue((DOM.Antlr.IValueNode) node), node.ValueType);
                return true;
            }

            return false;
        }

        private static bool IsArrayNode(IEnumerable<Entity> entities)
        {
            string firstEntityName = entities.FirstOrDefault()?.Name;
            return (firstEntityName == null) || (firstEntityName == string.Empty);
        }

        private static bool IsArrayNode(Element node)
        {
            if (node.Attributes.Count > 0) return false;
            return IsArrayNode(node.Entities);
        }
        private static bool IsArrayNode(Document node)
        {
            return IsArrayNode(node.Entities);
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
