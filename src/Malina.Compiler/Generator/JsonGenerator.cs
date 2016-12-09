using System;
using System.Collections.Generic;
using System.Linq;
using Malina.DOM;
using Newtonsoft.Json;
using Attribute = Malina.DOM.Attribute;
using ValueType = Malina.DOM.ValueType;

namespace Malina.Compiler
{
    public class JsonGenerator : MalinaDepthFirstVisitor
    {
        
        private readonly Func<string, JsonWriter> _writerDelegate;

        private JsonWriter _jsonWriter;

        /// <summary>
        /// Delegate will be called for the each Document. The name of the document will be sent in the string argument.
        /// </summary>
        /// <param name="writerDelegate"></param>
        public JsonGenerator(Func<string, JsonWriter> writerDelegate)
        {
            _writerDelegate = writerDelegate;
        }

        public override void OnDocument(Document node)
        {
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
        }


        public virtual void OnValue(object value, ValueType type)
        {
            if (type == ValueType.Null)
                _jsonWriter.WriteNull();
            else
            {
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
        }

        public override void OnAttribute(Attribute node)
        {
            _jsonWriter.WritePropertyName(node.Name);
            OnValue(node.Value, node.ValueType);
        }

        public override void OnElement(Element node)
        {
            if (node.Name != string.Empty)
                _jsonWriter.WritePropertyName(node.Name);
            var valueElement = node.Value != null || node.ObjectValue != null;
            if (valueElement)
                OnValue(node.Value, node.ValueType);
            else if (IsArrayNode(node))
            {
                _jsonWriter.WriteStartArray();
                base.OnElement(node);
                _jsonWriter.WriteEndArray();
            }
            else if (node.Attributes.Any() || node.Entities.Any() || node.ValueType == ValueType.Empty)
            {
                _jsonWriter.WriteStartObject();
                base.OnElement(node);
                _jsonWriter.WriteEndObject();
            }
            else if (node.Name != null)
            {
                _jsonWriter.WriteStartObject();
                _jsonWriter.WriteEndObject();
            }
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

    }
}
