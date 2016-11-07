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
        private bool _inDocument = false;

        public AliasResolvingListener(CompilerContext context)
        {
            _context = context;
            _nodeStack.Push(_context.CompileUnit);
        }

        protected override void EnterContext<T>(INodeContext<T> context, bool valueNode = false)
        {
            if (_inDocument) return;
            base.EnterContext(context, valueNode);
        }

        protected override void ExitContext<T>(INodeContext<T> context)
        {
            if (_inDocument) return;
            base.ExitContext(context);
        }

        public override void ExitString_value_inline( MalinaParser.String_value_inlineContext context)
        {
            if (_inDocument) return;
            base.ExitString_value_inline(context);
        }

        public override void ExitString_value_ml(MalinaParser.String_value_mlContext context)
        {
            if (_inDocument) return;
            base.ExitString_value_ml(context);
        }

        public override void EnterString_value_inline(MalinaParser.String_value_inlineContext context)
        {
            if (_inDocument) return;
            base.EnterString_value_inline(context);
        }

        public override void EnterString_value_ml(MalinaParser.String_value_mlContext context)
        {
            if (_inDocument) return;
            base.EnterString_value_ml(context);
        }


        protected override void EnterScopeContext(ParserRuleContext context)
        {
            if (_inDocument) return;
            base.EnterScopeContext(context);
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