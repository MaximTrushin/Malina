using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Malina.DOM;
using Malina.DOM.Antlr;
using System.Collections.Generic;
using IValueNode = Malina.DOM.IValueNode;
using Scope = Malina.DOM.Antlr.Scope;

namespace Malina.Parser
{
    /// <summary>
    /// Creates Malina DOM structure.
    /// </summary>
    public class MalinaParserListener: MalinaParserBaseListener
    {
        #region Class members
        private CompileUnit _compileUnit;
        protected Stack<Node> _nodeStack = new Stack<Node>();


        public MalinaParserListener(CompileUnit compileUnit)
        {
            _compileUnit = compileUnit;
            _nodeStack.Push(_compileUnit);
        }

        public MalinaParserListener(): this(new CompileUnit())
        {
        }

        public CompileUnit CompileUnit
        {
            get
            {
                return _compileUnit;
            }
        }
        protected virtual void EnterContext<T>(INodeContext<T> context, bool valueNode = false) where T : Node, new()
        {
            if (!valueNode)
                context.InitNode(_nodeStack.Count == 0 ? null : _nodeStack.Peek());
            else
                context.InitValueNode(_nodeStack.Count == 0 ? null : _nodeStack.Peek());
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
                var element = new DOM.Antlr.Element();

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

        #region ARRAY_ITEM context classes
        #region STATEMENT context 

        public override void EnterBlock_array_item_stmt([NotNull] MalinaParser.Block_array_item_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitBlock_array_item_stmt([NotNull] MalinaParser.Block_array_item_stmtContext context)
        {
            ExitContext(context);
        }


        #endregion
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
        }

        public override void EnterValue_parameter_stmt([NotNull] MalinaParser.Value_parameter_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitValue_parameter_stmt([NotNull] MalinaParser.Value_parameter_stmtContext context)
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
            var parent = (DOM.Antlr.IValueNode)_nodeStack.Peek();

            var dqs = context.DQS();
            if (dqs != null)
            {
                
                parent.ValueInterval = new Interval(((CommonToken) dqs.Payload).StartIndex + 1, ((CommonToken) dqs.Payload).StopIndex - 1);
                ((IValueNode) parent).ValueType = DOM.ValueType.DoubleQuotedString;
                return;
            }

            var openValue = context.OPEN_VALUE();
            if (openValue != null)
            {
                parent.ValueInterval = new Interval(((CommonToken) openValue.Payload).StartIndex, ((CommonToken) openValue.Payload).StopIndex);
                ((IValueNode) parent).ValueType = DOM.ValueType.OpenString;
                return;
            }

            var sqs = context.SQS();
            if (sqs != null)
            {
                parent.ValueInterval = new Interval(((CommonToken)sqs.Payload).StartIndex + 1, ((CommonToken)sqs.Payload).StopIndex - 1);
                ((IValueNode)parent).ValueType = DOM.ValueType.SingleQuotedString;
                return;
            }
        }

        public override void ExitString_value_ml([NotNull] MalinaParser.String_value_mlContext context)
        {
            var parent = (DOM.Antlr.IValueNode) _nodeStack.Peek();
            var open_value = context.OPEN_VALUE_ML();
            if (open_value != null)
            {
                var token = (MalinaToken) open_value.Payload;
                parent.ValueInterval = new Interval(token.StartIndex, token.StopIndex);
                parent.ValueIndent = token.TokenIndent;
                ((IValueNode) parent).ValueType = DOM.ValueType.OpenString;
                return;

            }

            var dqs_ml = context.DQS_ML();
            if (dqs_ml != null)
            {
                var token = (MalinaToken) dqs_ml.Payload;
                parent.ValueInterval = new Interval(token.StartIndex + 1, token.StopIndex - 1);
                parent.ValueIndent = token.TokenIndent;
                ((IValueNode) parent).ValueType = DOM.ValueType.DoubleQuotedString;
                return;
            }

            var sqs_ml = context.SQS_ML();
            if (sqs_ml != null)
            {
                var token = (MalinaToken) sqs_ml.Payload;
                parent.ValueInterval = new Interval(token.StartIndex + 1, token.StopIndex - 1);
                parent.ValueIndent = token.TokenIndent;
                ((IValueNode) parent).ValueType = DOM.ValueType.DoubleQuotedString;
                return;
            }
        }

        public override void EnterParameter_object_value_inline([NotNull] MalinaParser.Parameter_object_value_inlineContext context)
        {
            EnterContext(context, true);
        }

        public override void ExitParameter_object_value_inline([NotNull] MalinaParser.Parameter_object_value_inlineContext context)
        {
            ExitContext(context);
        }

        public override void EnterAlias_object_value_inline([NotNull] MalinaParser.Alias_object_value_inlineContext context)
        {
            EnterContext(context, true);
        }

        public override void ExitAlias_object_value_inline([NotNull] MalinaParser.Alias_object_value_inlineContext context)
        {
            ExitContext(context);
        }

        #endregion
    }
}
