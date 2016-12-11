using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Atn;
using Malina.DOM;
using Newtonsoft.Json;
using Attribute = Malina.DOM.Attribute;
using ValueType = Malina.DOM.ValueType;

namespace Malina.Compiler.Generator
{
    public class JsonGenerator : MalinaGenerator
    {
        private enum BlockState
        {
            Object,
            Array
        }

        private readonly Func<string, JsonWriter> _writerDelegate;

        private JsonWriter _jsonWriter;
        private bool _blockStart;
        private Stack<BlockState> _blockState;


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
                _blockStart = true;
                _blockState = new Stack<BlockState>();
                base.OnDocument(node);
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
            CheckBlockStart(node);

            _jsonWriter.WritePropertyName((node.NsPrefix != null ? node.NsPrefix + "." : "") + node.Name);
            ResolveValue(node);
        }

        public override void OnElement(Element node)
        {
            CheckBlockStart(node);

            if (!string.IsNullOrEmpty(node.Name))
                _jsonWriter.WritePropertyName((node.NsPrefix != null ? node.NsPrefix + "." : "") + node.Name);

            if (ResolveValue(node)) return;

            //Working with node's block
            _blockStart = true;
            var prevBlockStateCount = _blockState.Count;

            ResolveAttributes(node.Attributes, node.Entities);
            Visit(node.Entities);
            _blockStart = false;

            if (_blockState.Count > prevBlockStateCount)
            {
                if (_blockState.Pop() == BlockState.Array)
                    _jsonWriter.WriteEndArray();
                else
                    _jsonWriter.WriteEndObject();
                //return;
            }


            //if (node.Name != null)
            //{
            //    _jsonWriter.WriteStartObject();
            //    _jsonWriter.WriteEndObject();
            //    //return;
            //}
        }

        private void CheckBlockStart(Node node)
        {
            if (_blockStart)
            {
                //This element is the first element of the block. It decides if the block is array or object
                if (string.IsNullOrEmpty(node.Name))
                {
                    _jsonWriter.WriteStartArray(); //start array
                    _blockState.Push(BlockState.Array);
                }
                else
                {
                    _jsonWriter.WriteStartObject(); //start array
                    _blockState.Push(BlockState.Object);
                }
                _blockStart = false;
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


        public override void OnAliasDefinition(AliasDefinition node)
        {
            //Doing nothing for Alias Definition        
        }


    }
}
