using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Malina.DOM;
using Malina.DOM.Antlr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malina.Parser
{
    public class MalinaParserListener: MalinaParserBaseListener
    {
        #region Class members
        private List<Node> _nodes = new List<Node>();
        private Stack<Node> _nodeStack = new Stack<Node>();

        public List<Node> Nodes
        {
            get
            {
                return _nodes;
            }
        }
        private void EnterContext<T>(INodeContext<T> context, bool valueNode = false) where T : Node, IAntlrCharStreamConsumer, new()
        {
            if(!valueNode)
                context.InitNode(_nodeStack.Count == 0 ? null : _nodeStack.Peek());
            else
                context.InitValueNode(_nodeStack.Count == 0 ? null : _nodeStack.Peek());

            if (_nodeStack.Count == 0) _nodes.Add(context.Node);

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

        private void EnterScopeContext(ParserRuleContext context)
        {
            //Creating Scope node, adding to parent, adding to ctx.Node and initializing CharStream
            (context as INodeContext<DOM.Antlr.Scope>).InitNode(_nodeStack.Count == 0 ? null : _nodeStack.Peek());

            //Checking if this is root node and retuning as Listener result
            if (_nodeStack.Count == 0) _nodes.Add((context as INodeContext<DOM.Antlr.Scope>).Node);


            var first = context.Start.Text;

            var dot = FindChar(context.Start.StartIndex, context.Start.StopIndex, context.Start.InputStream, '.');
            if (dot > -1)
            {
                //SCOPE_ID found. Need to create SCOPE and ELEMENT

                //Initializing ELEMENT
                var element = new DOM.Antlr.Element();

                (context as INodeContext<DOM.Antlr.Scope>).Node.AppendChild(element);

                element.CharStream = context.Start.InputStream;

                _nodeStack.Push(element); //Adding element to node stack. 

            }
            else
            {
                //NAMESPACE_ID found. Need to create SCOPE only

                _nodeStack.Push((context as INodeContext<DOM.Antlr.Scope>).Node);
            }
        }

        private void ExitScopeContext(ParserRuleContext context)
        {

        }

        private void ExitContext<T>(INodeContext<T> context) where T : Node, new()
        {
            context.ApplyContext();
            _nodeStack.Pop();
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
            var parent = _nodeStack.Peek() as IValueNode;

            var dqs = context.dqs_inline();
            if (dqs != null)
            {
                parent.ValueIntervals = new List<Interval>();
                if((dqs.GetChild(1).Payload as CommonToken).Type == MalinaParser.DQS_VALUE)
                parent.ValueIntervals.Add(new Interval((dqs.GetChild(1).Payload as CommonToken).StartIndex, (dqs.GetChild(1).Payload as CommonToken).StopIndex));
                return;
            }

            var openValue = context.OPEN_VALUE();
            if (openValue != null)
            {
                parent.ValueIntervals = new List<Interval>();
                parent.ValueIntervals.Add(new Interval((openValue.Payload as CommonToken).StartIndex, (openValue.Payload as CommonToken).StopIndex));

            }

        }

        public override void ExitString_value_ml([NotNull] MalinaParser.String_value_mlContext context)
        {
            var parent = _nodeStack.Peek() as IValueNode;
            var open_value = context.children[1] as MalinaParser.Open_value_mlContext;
            if (open_value != null)
            {
                parent.ValueIntervals = new List<Interval>();
                var previousIsIndent = true;
                foreach (var item in open_value.children)
                {
                    if((item.Payload as CommonToken).Type == MalinaParser.OPEN_VALUE)
                    {
                        if(!previousIsIndent) parent.ValueIntervals.Add(new Interval(-1, -1));//Adding New Line
                        parent.ValueIntervals.Add(new Interval((item.Payload as CommonToken).StartIndex, (item.Payload as CommonToken).StopIndex));
                        previousIsIndent = false;
                    }
                    else
                    {//OPEN_VALUE_INDENT
                        parent.ValueIntervals.Add(new Interval(-1, -1));//Adding New Line
                        parent.ValueIntervals.Add(new Interval((item.Payload as CommonToken).StartIndex, (item.Payload as CommonToken).StopIndex));
                        previousIsIndent = true;
                    }

                }
            }
            var dqs_ml = context.children[1] as MalinaParser.Dqs_mlContext;
            if (dqs_ml != null)
            {
                parent.ValueIntervals = new List<Interval>();
                var previousIsIndent = true;
                foreach (var item in dqs_ml.children)
                {
                    if ((item.Payload as CommonToken).Type == MalinaParser.DQS_VALUE)
                    {
                        if (!previousIsIndent) parent.ValueIntervals.Add(new Interval(-1, -1));//Adding New Line
                        parent.ValueIntervals.Add(new Interval((item.Payload as CommonToken).StartIndex, (item.Payload as CommonToken).StopIndex));
                        previousIsIndent = false;
                    }
                    else if ((item.Payload as CommonToken).Type == MalinaParser.DQS_VALUE_EOL)
                    {//DQS_VALUE_EOL
                        parent.ValueIntervals.Add(new Interval(-1, -1));//Adding New Line
                        parent.ValueIntervals.Add(new Interval((item.Payload as CommonToken).StartIndex, (item.Payload as CommonToken).StopIndex));
                        previousIsIndent = true;
                    }

                }
            }


        }

        public override void EnterParameter_object_value_inline([NotNull] MalinaParser.Parameter_object_value_inlineContext context)
        {
            EnterContext(context);
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
