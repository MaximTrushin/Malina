using Antlr4.Runtime;
using Malina.DOM;
using Malina.DOM.Antlr;
using Malina.Parser;
using System.Collections.Generic;

namespace Malina.Compiler.Steps
{
    public class AliasResolvingListener : MalinaParserListener
    {
        #region CLASS members
        private CompilerContext _context;
        private Stack<Node> _nodeStack = new Stack<Node>();
        private bool _inDocument = false;

        public AliasResolvingListener(CompilerContext context)
        {
            _context = context;
            _nodeStack.Push(_context.CompileUnit);
        }

        protected override void EnterContext<T>(INodeContext<T> context, bool valueNode = false)
        {
            if (_inDocument) return;
            if (!valueNode)
                context.InitNode(_nodeStack.Count == 0 ? null : _nodeStack.Peek());
            else
                context.InitValueNode(_nodeStack.Count == 0 ? null : _nodeStack.Peek());

            _nodeStack.Push(context.Node);
        }

        protected override void ExitContext<T>(INodeContext<T> context)
        {
            if (_inDocument) return;
            base.ExitContext(context);
        }

        protected override void EnterScopeContext(ParserRuleContext context)
        {
            //Creating Scope node, adding to parent, adding to ctx.Node and initializing CharStream
            (context as INodeContext<DOM.Antlr.Scope>).InitNode(_nodeStack.Count == 0 ? null : _nodeStack.Peek());

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

        #endregion

        #region MODULE context classes
        public override void EnterModule(MalinaParser.ModuleContext context)
        {
            EnterContext(context);
        }

        public override void ExitModule(MalinaParser.ModuleContext context)
        {
            ExitContext(context);
        }
        #endregion


        #region DOCUMENT context classes
        public override void EnterDocument_stmt(MalinaParser.Document_stmtContext context)
        {
            _inDocument = true;
        }

        public override void ExitDocument_stmt(MalinaParser.Document_stmtContext context)
        {
            _inDocument = false;
        }
        #endregion
    }
}