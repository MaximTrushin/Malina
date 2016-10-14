using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
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
        private void EnterContext<T>(INodeContext<T> context) where T : Node, IAntlrCharStreamConsumer, new()
        {
            context.InitNode(_nodeStack.Count == 0 ? null : _nodeStack.Peek());

            if (_nodeStack.Count == 0) _nodes.Add(context.Node);

            _nodeStack.Push(context.Node);
        }

        private void ExitContext<T>(INodeContext<T> context) where T : Node, new()
        {
            context.ApplyContext();
            _nodeStack.Pop();
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

        #region PARAMETER context classes
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

        #region ALIAS context classes
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


    }
}
