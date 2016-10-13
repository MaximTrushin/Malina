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
        private List<Node> _nodes = new List<Node>();
        private Stack<Node> _nodeStack = new Stack<Node>();

        public List<Node> Nodes
        {
            get
            {
                return _nodes;
            }
        }

        public override void EnterAlias_def_stmt([NotNull] MalinaParser.Alias_def_stmtContext context)
        {
            EnterContext(context);
            base.EnterAlias_def_stmt(context);
        }


        public override void ExitAlias_def_stmt([NotNull] MalinaParser.Alias_def_stmtContext context)
        {
            base.ExitAlias_def_stmt(context);
            ExitContext(context);
        }

        public override void EnterAttr_stmt([NotNull] MalinaParser.Attr_stmtContext context)
        {
            EnterContext(context);
        }

        public override void ExitAttr_stmt([NotNull] MalinaParser.Attr_stmtContext context)
        {
            ExitContext(context);
        }

        public override void EnterAttr_inline([NotNull] MalinaParser.Attr_inlineContext context)
        {
            EnterContext(context);
        }
 
        public override void ExitAttr_inline([NotNull] MalinaParser.Attr_inlineContext context)
        {
            ExitContext(context);
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

    }
}
