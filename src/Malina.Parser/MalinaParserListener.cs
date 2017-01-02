using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Malina.DOM;
using Alias = Malina.DOM.Antlr.Alias;
using Element = Malina.DOM.Antlr.Element;
using IValueNode = Malina.DOM.Antlr.IValueNode;
using Scope = Malina.DOM.Antlr.Scope;

namespace Malina.Parser
{
    /// <summary>
    /// Creates Malina DOM structure.
    /// </summary>
    public class MalinaParserListener: MalinaParserBaseListener
    {
        #region Class members
        //CompileUnit where all Modules will be added
        private readonly CompileUnit _compileUnit;

       protected readonly Stack<Node> _nodeStack = new Stack<Node>();


        public MalinaParserListener(CompileUnit compileUnit)
        {
            _compileUnit = compileUnit;
            _nodeStack.Push(_compileUnit);
        }

        public MalinaParserListener(): this(new CompileUnit())
        {
        }

        public CompileUnit CompileUnit => _compileUnit;

        protected virtual void EnterContext<T>(INodeContext<T> context, bool valueNode = false) where T : Node, new()
        {
            var valueNodeExpected = false;
            var parent = _nodeStack.Peek();
            if (parent is DOM.IValueNode)
            {
                valueNodeExpected = (parent as DOM.IValueNode).ValueType == ValueType.ObjectValue;
            }

            if (!(valueNode || valueNodeExpected))
                context.InitNode(_nodeStack.Count == 0 ? null : parent);
            else
                context.InitValueNode(_nodeStack.Count == 0 ? null : parent);
            _nodeStack.Push(context.Node);
        }

        public static int FindChar(int start, int stop, ICharStream input, char c)
        {
            for (int i = start - input.Index + 1; i <= stop - input.Index; i++)
            {
                if (input.La(i) == c) return input.Index + i;
            }
            return -1;
        }

        protected virtual void EnterScopeContext(ParserRuleContext context)
        {
            //Creating Scope node, adding to parent, adding to ctx.Node and initializing CharStream
            (context as INodeContext<Scope>).InitNode(_nodeStack.Count == 0 ? null : _nodeStack.Peek());

            //Checking if this is root node and retuning as Listener result

            var dot = FindChar(context.Start.StartIndex, context.Start.StopIndex, context.Start.InputStream, '.');
            if (dot > -1)
            {
                //SCOPE_ID found. Need to create SCOPE and ELEMENT

                //Initializing ELEMENT
                var element = new Element();

                ((INodeContext<Scope>) context).Node.AppendChild(element);

                element.CharStream = context.Start.InputStream;

                _nodeStack.Push(element); //Adding element to node stack. 

            }
            else
            {
                //NAMESPACE_ID found. Need to create SCOPE only

                _nodeStack.Push(((INodeContext<Scope>) context).Node);
            }
        }


        protected virtual void ExitContext<T>(INodeContext<T> context) where T : Node, new()
        {
            context.ApplyContext();
            _nodeStack.Pop();
        }
        #endregion

        #region MODULE context classes
        public override void EnterModule([NotNull] MalinaParser.ModuleContext context)
        {
            EnterContext(context);
        }

        public override void ExitModule([NotNull] MalinaParser.ModuleContext context)
        {
            ExitContext(context);
        }
        #endregion

        #region DOCUMENT context classes
        public override void EnterDocument_stmt([NotNull] MalinaParser.Document_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitDocument_stmt([NotNull] MalinaParser.Document_stmtContext context)
        {
            ExitContext(context);
        }

        public override void EnterNamespace_declaration_stmt([NotNull] MalinaParser.Namespace_declaration_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitNamespace_declaration_stmt([NotNull] MalinaParser.Namespace_declaration_stmtContext context)
        {
            ExitContext(context);
        }

        public override void EnterScope_stmt([NotNull] MalinaParser.Scope_stmtContext context)
        {
            context.NodeStack = _nodeStack;
            EnterScopeContext(context);
        }

        public override void ExitScope_stmt([NotNull] MalinaParser.Scope_stmtContext context)
        {            
            ExitContext(context);
        }

        public override void EnterScope_inline([NotNull] MalinaParser.Scope_inlineContext context)
        {
            context.NodeStack = _nodeStack;
            EnterScopeContext(context);
        }

        public override void ExitScope_inline([NotNull] MalinaParser.Scope_inlineContext context)
        {
            ExitContext(context);
        }
        #endregion

        #region ALIAS_DEF context classes
        public override void EnterAlias_def_stmt([NotNull] MalinaParser.Alias_def_stmtContext context)
        {
            EnterContext(context);
        }


        public override void ExitAlias_def_stmt([NotNull] MalinaParser.Alias_def_stmtContext context)
        {
            ExitContext(context);
        }
        #endregion

        #region ATTRIBUTE context classes
        public override void EnterAttr_stmt([NotNull] MalinaParser.Attr_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitAttr_stmt([NotNull] MalinaParser.Attr_stmtContext context)
        {
            ExitContext(context);
        }

        public override void EnterValue_attr_inline([NotNull] MalinaParser.Value_attr_inlineContext context)
        {
            EnterContext(context);
        }
 
        public override void ExitValue_attr_inline([NotNull] MalinaParser.Value_attr_inlineContext context)
        {
            ExitContext(context);
        }

        public override void EnterEmpty_attr_inline([NotNull] MalinaParser.Empty_attr_inlineContext context)
        {
            EnterContext(context);
        }

        public override void ExitEmpty_attr_inline([NotNull] MalinaParser.Empty_attr_inlineContext context)
        {
            ExitContext(context);
        }
        #endregion

        #region ELEMENT context classes
        #region STATEMENT context 
        public override void EnterValue_element_stmt([NotNull] MalinaParser.Value_element_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitValue_element_stmt([NotNull] MalinaParser.Value_element_stmtContext context)
        {
            ExitContext(context);
        }

        public override void EnterEmpty_element_stmt([NotNull] MalinaParser.Empty_element_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitEmpty_element_stmt([NotNull] MalinaParser.Empty_element_stmtContext context)
        {
            ExitContext(context);
        }

        public override void EnterBlock_element_stmt([NotNull] MalinaParser.Block_element_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitBlock_element_stmt([NotNull] MalinaParser.Block_element_stmtContext context)
        {
            ExitContext(context);
        }

        public override void EnterHybrid_block_element_stmt(MalinaParser.Hybrid_block_element_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitHybrid_block_element_stmt(MalinaParser.Hybrid_block_element_stmtContext context)
        {
            ExitContext(context);
        }

        #endregion

        #region INLINE context
        public override void EnterValue_element_inline([NotNull] MalinaParser.Value_element_inlineContext context)
        {
            EnterContext(context);
        }

        public override void ExitValue_element_inline([NotNull] MalinaParser.Value_element_inlineContext context)
        {
            ExitContext(context);
        }

        public override void EnterEmpty_element_inline([NotNull] MalinaParser.Empty_element_inlineContext context)
        {
            EnterContext(context);
        }

        public override void ExitEmpty_element_inline([NotNull] MalinaParser.Empty_element_inlineContext context)
        {
            ExitContext(context);
        }

        public override void EnterBlock_element_inline([NotNull] MalinaParser.Block_element_inlineContext context)
        {
            EnterContext(context);
        }

        public override void ExitBlock_element_inline([NotNull] MalinaParser.Block_element_inlineContext context)
        {
            ExitContext(context);
        }
        #endregion
        #endregion

        #region ARRAY_ITEM context classes
        #region STATEMENT context 

        public override void EnterValue_array_item_stmt(MalinaParser.Value_array_item_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitValue_array_item_stmt(MalinaParser.Value_array_item_stmtContext context)
        {
            ExitContext(context);
        }

        public override void EnterBlock_array_item_stmt(MalinaParser.Block_array_item_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitBlock_array_item_stmt(MalinaParser.Block_array_item_stmtContext context)
        {
            ExitContext(context);
        }

        public override void EnterHybrid_block_array_item_stmt(MalinaParser.Hybrid_block_array_item_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitHybrid_block_array_item_stmt(MalinaParser.Hybrid_block_array_item_stmtContext context)
        {
            ExitContext(context);
        }

        public override void EnterEmpty_array_item_stmt(MalinaParser.Empty_array_item_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitEmpty_array_item_stmt(MalinaParser.Empty_array_item_stmtContext context)
        {
            ExitContext(context);
        }



        #endregion
        #region INLINE context
        public override void EnterValue_array_item_inline([NotNull] MalinaParser.Value_array_item_inlineContext context)
        {
            EnterContext(context);
        }
        public override void ExitValue_array_item_inline([NotNull] MalinaParser.Value_array_item_inlineContext context)
        {
            ExitContext(context);
        }

        public override void EnterBlock_array_item_inline([NotNull] MalinaParser.Block_array_item_inlineContext context)
        {
            EnterContext(context);
        }

        public override void ExitBlock_array_item_inline([NotNull] MalinaParser.Block_array_item_inlineContext context)
        {
            ExitContext(context);
        }

        public override void EnterEmpty_array_inline(MalinaParser.Empty_array_inlineContext context)
        {
            EnterContext(context);
        }

        public override void ExitEmpty_array_inline(MalinaParser.Empty_array_inlineContext context)
        {
            ExitContext(context);
        }

        #endregion
        #endregion

        #region EMPTY OBJECT

        public override void ExitEmpty_object_stmt(MalinaParser.Empty_object_stmtContext context)
        {
            var parent = _nodeStack.Peek() as DOM.IValueNode;
            if (parent != null)
            {
                parent.ValueType = ValueType.EmptyObject;
            }
        }

        public override void ExitEmpty_object_inline(MalinaParser.Empty_object_inlineContext context)
        {
            var parent = _nodeStack.Peek() as DOM.IValueNode;
            if (parent != null)
            {
                parent.ValueType = ValueType.EmptyObject;
            }
        }

        #endregion

        #region PARAMETER context classes
        #region STATEMENT context
        public override void EnterBlock_parameter_stmt([NotNull] MalinaParser.Block_parameter_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitBlock_parameter_stmt([NotNull] MalinaParser.Block_parameter_stmtContext context)
        {
            ExitContext(context);
        }

        public override void EnterEmpty_parameter_stmt([NotNull] MalinaParser.Empty_parameter_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitEmpty_parameter_stmt([NotNull] MalinaParser.Empty_parameter_stmtContext context)
        {
            ExitContext(context);
            //If there empty block then set node's ValueType to EmptyObject
            if (context.COLON() != null)
            {
                context.Node.ValueType = ValueType.EmptyObject;
            }

        }

        public override void EnterValue_parameter_stmt([NotNull] MalinaParser.Value_parameter_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitValue_parameter_stmt([NotNull] MalinaParser.Value_parameter_stmtContext context)
        {
            ExitContext(context);
        }

        public override void EnterHybrid_block_parameter_stmt(MalinaParser.Hybrid_block_parameter_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitHybrid_block_parameter_stmt(MalinaParser.Hybrid_block_parameter_stmtContext context)
        {
            ExitContext(context);
        }

        #endregion

        #region INLINE context
        public override void EnterBlock_parameter_inline([NotNull] MalinaParser.Block_parameter_inlineContext context)
        {
            EnterContext(context);
        }

        public override void ExitBlock_parameter_inline([NotNull] MalinaParser.Block_parameter_inlineContext context)
        {
            ExitContext(context);
        }

        public override void EnterEmpty_parameter_inline([NotNull] MalinaParser.Empty_parameter_inlineContext context)
        {
            EnterContext(context);
        }

        public override void ExitEmpty_parameter_inline([NotNull] MalinaParser.Empty_parameter_inlineContext context)
        {
            ExitContext(context);

            //If there empty block then set node's ValueType to EmptyObject
            if (context.COLON() != null)
            {
                context.Node.ValueType = ValueType.EmptyObject;
            }
        }

        public override void EnterValue_parameter_inline([NotNull] MalinaParser.Value_parameter_inlineContext context)
        {
            EnterContext(context);
        }

        public override void ExitValue_parameter_inline([NotNull] MalinaParser.Value_parameter_inlineContext context)
        {
            ExitContext(context);
        }
        #endregion
        #endregion

        #region ALIAS context classes
        #region STATEMENT context
        public override void EnterBlock_alias_stmt([NotNull] MalinaParser.Block_alias_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitBlock_alias_stmt([NotNull] MalinaParser.Block_alias_stmtContext context)
        {
            ExitContext(context);
        }

        public override void EnterHybrid_block_alias_stmt(MalinaParser.Hybrid_block_alias_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitHybrid_block_alias_stmt(MalinaParser.Hybrid_block_alias_stmtContext context)
        {
            ExitContext(context);
        }


        public override void EnterEmpty_alias_stmt([NotNull] MalinaParser.Empty_alias_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitEmpty_alias_stmt([NotNull] MalinaParser.Empty_alias_stmtContext context)
        {
            ExitContext(context);
        }

        public override void EnterValue_alias_stmt([NotNull] MalinaParser.Value_alias_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitValue_alias_stmt([NotNull] MalinaParser.Value_alias_stmtContext context)
        {
            ExitContext(context);
        }
        #endregion
        #region INLINE context
        public override void EnterBlock_alias_inline([NotNull] MalinaParser.Block_alias_inlineContext context)
        {
            EnterContext(context);
        }

        public override void ExitBlock_alias_inline([NotNull] MalinaParser.Block_alias_inlineContext context)
        {
            ExitContext(context);
        }

        public override void EnterEmpty_alias_inline([NotNull] MalinaParser.Empty_alias_inlineContext context)
        {
            EnterContext(context);
        }

        public override void ExitEmpty_alias_inline([NotNull] MalinaParser.Empty_alias_inlineContext context)
        {
            ExitContext(context);
        }

        public override void EnterValue_alias_inline([NotNull] MalinaParser.Value_alias_inlineContext context)
        {
            EnterContext(context);
        }

        public override void ExitValue_alias_inline([NotNull] MalinaParser.Value_alias_inlineContext context)
        {
            ExitContext(context);
        }
        #endregion
        #endregion

        #region ARGUMENT context classes
        #region STATEMENT context
        public override void EnterBlock_argument_stmt([NotNull] MalinaParser.Block_argument_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitBlock_argument_stmt([NotNull] MalinaParser.Block_argument_stmtContext context)
        {
            ExitContext(context);
        }

        public override void EnterEmpty_argument_stmt([NotNull] MalinaParser.Empty_argument_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitEmpty_argument_stmt([NotNull] MalinaParser.Empty_argument_stmtContext context)
        {
            ExitContext(context);
        }

        public override void EnterValue_argument_stmt([NotNull] MalinaParser.Value_argument_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitValue_argument_stmt([NotNull] MalinaParser.Value_argument_stmtContext context)
        {
            ExitContext(context);
        }

        public override void EnterHybrid_block_argument_stmt(MalinaParser.Hybrid_block_argument_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitHybrid_block_argument_stmt(MalinaParser.Hybrid_block_argument_stmtContext context)
        {
            ExitContext(context);
        }

        #endregion

        #region INLINE context
        public override void EnterBlock_argument_inline([NotNull] MalinaParser.Block_argument_inlineContext context)
        {
            EnterContext(context);
        }

        public override void ExitBlock_argument_inline([NotNull] MalinaParser.Block_argument_inlineContext context)
        {
            ExitContext(context);
        }

        public override void EnterEmpty_argument_inline([NotNull] MalinaParser.Empty_argument_inlineContext context)
        {
            EnterContext(context);
        }

        public override void ExitEmpty_argument_inline([NotNull] MalinaParser.Empty_argument_inlineContext context)
        {
            ExitContext(context);
        }

        public override void EnterValue_argument_inline([NotNull] MalinaParser.Value_argument_inlineContext context)
        {
            EnterContext(context);
        }

        public override void ExitValue_argument_inline([NotNull] MalinaParser.Value_argument_inlineContext context)
        {
            ExitContext(context);
        }
        #endregion
        #endregion

        #region Value
        public override void ExitString_value_inline([NotNull] MalinaParser.String_value_inlineContext context)
        {
            var parent = (IValueNode)_nodeStack.Peek();

            var dqs = context.DQS();
            if (dqs != null)
            {
                
                parent.ValueInterval = new Interval(((CommonToken) dqs.Payload).StartIndex + 1, ((CommonToken) dqs.Payload).StopIndex - 1);
                ((DOM.IValueNode) parent).ValueType = ValueType.DoubleQuotedString;
                return;
            }

            var openValue = context.OPEN_STRING();
            if (openValue != null)
            {
                parent.ValueInterval = new Interval(((CommonToken) openValue.Payload).StartIndex, ((CommonToken) openValue.Payload).StopIndex);
                ((DOM.IValueNode) parent).ValueType = ValueType.OpenString;
                return;
            }

            var freeOpenValue = context.FREE_OPEN_STRING();
            if (freeOpenValue != null)
            {
                parent.ValueInterval = new Interval(((CommonToken)freeOpenValue.Payload).StartIndex, ((CommonToken)freeOpenValue.Payload).StopIndex);
                ((DOM.IValueNode)parent).ValueType = ValueType.FreeOpenString;
                return;
            }

            var jsonBoolean = context.JSON_BOOLEAN();
            if (jsonBoolean != null)
            {
                parent.ValueInterval = new Interval(((CommonToken)jsonBoolean.Payload).StartIndex, ((CommonToken)jsonBoolean.Payload).StopIndex);
                ((DOM.IValueNode)parent).ValueType = ValueType.Boolean;
                return;
            }

            var jsonNull = context.JSON_NULL();
            if (jsonNull != null)
            {
                parent.ValueInterval = new Interval(((CommonToken)jsonNull.Payload).StartIndex, ((CommonToken)jsonNull.Payload).StopIndex);
                ((DOM.IValueNode)parent).ValueType = ValueType.Null;
                return;
            }

            var jsonNumber = context.JSON_NUMBER();
            if (jsonNumber != null)
            {
                parent.ValueInterval = new Interval(((CommonToken)jsonNumber.Payload).StartIndex, ((CommonToken)jsonNumber.Payload).StopIndex);
                ((DOM.IValueNode)parent).ValueType = ValueType.Number;
                return;
            }

            var sqs = context.sqs_inline();
            if (sqs != null)
            {
                SetInterpolationItems(parent, sqs.children);

                //Set ValueInterval
                var stopIndex = ((CommonToken)sqs.Stop).Type == MalinaLexer.SQS_END ? ((CommonToken)sqs.Stop).StopIndex - 1 : ((CommonToken)sqs.Stop).StopIndex;
                parent.ValueInterval = new Interval(((CommonToken)sqs.Start).StartIndex + 1, stopIndex);

                //Set Indent
                parent.ValueIndent = ((MalinaToken)sqs.children[0].Payload).TokenIndent;

                //Set Value type to SingleQuotedString
                ((DOM.IValueNode)parent).ValueType = ValueType.SingleQuotedString;
            }

            var jsonLiteral = context.sqs_json_literal();
            if (jsonLiteral != null)
            {
                //Set ValueInterval
                var stopIndex = ((CommonToken)jsonLiteral.Stop).Type == MalinaLexer.SQS_END ? ((CommonToken)jsonLiteral.Stop).StopIndex - 1 : ((CommonToken)jsonLiteral.Stop).StopIndex;
                parent.ValueInterval = new Interval(((CommonToken)jsonLiteral.Start).StartIndex + 1, stopIndex);
                var literalToken = (CommonToken)jsonLiteral.children[1].Payload;
                switch(literalToken.Type)
                {
                    case MalinaLexer.SQS_JSON_BOOLEAN:
                        ((DOM.IValueNode)parent).ValueType = ValueType.Boolean;
                        break;
                    case MalinaLexer.SQS_JSON_NULL:
                        ((DOM.IValueNode)parent).ValueType = ValueType.Null;
                        break;
                    case MalinaLexer.SQS_JSON_NUMBER:
                        ((DOM.IValueNode)parent).ValueType = ValueType.Number;
                        break;
                }

            }
        }

        private static void SetInterpolationItems(IValueNode node, IList<IParseTree> children)
        {

            foreach (var item in children)
            {
                var token = item.Payload as CommonToken;
                if (token != null)
                {
                    //Skipping quotes
                    if (token.Type == MalinaLexer.SQS || token.Type == MalinaLexer.SQS_END) continue;

                    //Adding SQL_EOL
                    node.InterpolationItems.Add(token);
                    continue;
                }

                //Processing tokens from sqs_body_item
                var ruleContext = item as MalinaParser.Sqs_body_itemContext;

                if (ruleContext == null) continue;

                foreach (var parseTree in ruleContext.children)
                {
                    token = (CommonToken)parseTree.Payload;

                    if (token.Type == MalinaLexer.INTERPOLATION)
                    {
                        var aliasName = token.Text.TrimStart('$', '(', '\t', ' ').TrimEnd(')', '\t', ' ');

                        node.InterpolationItems.Add(
                            new Alias
                            {
                                Name = aliasName,
                                start = new SourceLocation(token.Line, token.Column + 1, token.StartIndex),
                                end = new SourceLocation(token.Line, token.Column + 1, token.StartIndex),
                                ValueType = ValueType.Empty
                            }
                        );
                    }
                    else
                    {
                        node.InterpolationItems.Add(token);
                    }
                }
            }
        }

        public override void ExitString_value_ml([NotNull] MalinaParser.String_value_mlContext context)
        {
            var parent = (IValueNode) _nodeStack.Peek();
            var open_value = context.OPEN_STRING_ML();
            if (open_value != null)
            {
                var token = (MalinaToken) open_value.Payload;
                parent.ValueInterval = new Interval(token.StartIndex, token.StopIndex);
                parent.ValueIndent = token.TokenIndent;
                ((DOM.IValueNode) parent).ValueType = ValueType.OpenString;
                return;
            }

            var free_open_value = context.FREE_OPEN_STRING_ML();
            if (free_open_value != null)
            {
                var token = (MalinaToken)free_open_value.Payload;
                parent.ValueInterval = new Interval(token.StartIndex, token.StopIndex);
                parent.ValueIndent = token.TokenIndent;
                ((DOM.IValueNode)parent).ValueType = ValueType.FreeOpenString;
                return;
            }

            var dqs_ml = context.DQS_ML();
            if (dqs_ml != null)
            {
                var token = (MalinaToken) dqs_ml.Payload;
                parent.ValueInterval = new Interval(token.StartIndex + 1, token.StopIndex - 1);
                parent.ValueIndent = token.TokenIndent;
                ((DOM.IValueNode) parent).ValueType = ValueType.DoubleQuotedString;
                return;
            }

            var sqs_ml = context.sqs_ml();
            if (sqs_ml != null)
            {

                SetInterpolationItems(parent, sqs_ml.children);

                //Set ValueInterval
                var stopIndex = ((CommonToken)sqs_ml.Stop).Type == MalinaLexer.SQS_END ? ((CommonToken)sqs_ml.Stop).StopIndex - 1: ((CommonToken)sqs_ml.Stop).StopIndex;
                parent.ValueInterval = new Interval(((CommonToken)sqs_ml.Start).StartIndex + 1, stopIndex);

                //Set Indent
                parent.ValueIndent = ((MalinaToken)sqs_ml.children[0].Payload).TokenIndent;

                //Set Value type to SingleQuotedString
                ((DOM.IValueNode)parent).ValueType = ValueType.SingleQuotedString;

            }
        }

        public override void EnterObject_value(MalinaParser.Object_valueContext context)
        {
            SetParentsValueType(ValueType.ObjectValue);
        }

        public override void EnterObject_value_inline(MalinaParser.Object_value_inlineContext context)
        {
            SetParentsValueType(ValueType.ObjectValue);
        }

        #endregion

        private void SetParentsValueType(ValueType valueType)
        {
            var parent = _nodeStack.Peek() as DOM.IValueNode;
            if (parent != null) parent.ValueType = valueType;
        }
    }
}
