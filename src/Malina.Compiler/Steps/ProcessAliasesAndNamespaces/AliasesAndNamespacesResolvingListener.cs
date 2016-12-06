using Antlr4.Runtime;
using Malina.DOM;
using Malina.DOM.Antlr;
using Malina.Parser;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using System;
using System.Linq;
using Alias = Malina.DOM.Antlr.Alias;
using IValueNode = Malina.DOM.Antlr.IValueNode;

namespace Malina.Compiler.Steps
{
    /// <summary>
    /// Creates Malina DOM structure (because it is inherited from MalinaParserListener).
    /// Populates list of AliasDefinitions in the CompilerContext.
    /// Resolves namespaces for documents
    /// </summary>
    public class AliasesAndNamespacesResolvingListener : MalinaParserListener
    {
        private readonly CompilerContext _context;
        private readonly string _fileName;

        public AliasesAndNamespacesResolvingListener(CompilerContext context, string fileName) : base(context.CompileUnit)
        {
            _context = context;
            _fileName = fileName;
        }

        public override void EnterModule(MalinaParser.ModuleContext context)
        {
            base.EnterModule(context);
            context.Node.FileName = _fileName;
            _context.NamespaceResolver.EnterModule(context.Node);
        }

        public override void EnterDocument_stmt([NotNull] MalinaParser.Document_stmtContext context)
        {            
            base.EnterDocument_stmt(context);
            _context.NamespaceResolver.EnterDocument(context.Node);
        }

        public override void ExitDocument_stmt(MalinaParser.Document_stmtContext context)
        {
            base.ExitDocument_stmt(context);
            _context.NamespaceResolver.ExitDocument(context.Node);
        }

        public override void EnterAlias_def_stmt([NotNull] MalinaParser.Alias_def_stmtContext context)
        {
            base.EnterAlias_def_stmt(context);
            _context.NamespaceResolver.EnterAliasDef(context.Node);
        }

        public override void ExitAlias_def_stmt(MalinaParser.Alias_def_stmtContext context)
        {
            base.ExitAlias_def_stmt(context);
            _context.NamespaceResolver.ExitAliasDef(context.Node);
        }

        public override void ExitEveryRule([NotNull] ParserRuleContext context)
        {
            base.ExitEveryRule(context);

            if (context is INodeContext<DOM.Antlr.Element>) _context.NamespaceResolver.ProcessNsPrefix(((INodeContext<DOM.Antlr.Element>) context).Node);
            else if (context is INodeContext<DOM.Antlr.Attribute>) _context.NamespaceResolver.ProcessNsPrefix(((INodeContext<DOM.Antlr.Attribute>) context).Node);
            else if (context is INodeContext<DOM.Antlr.Alias>) _context.NamespaceResolver.ProcessAlias(((INodeContext<DOM.Antlr.Alias>) context).Node);
            else if (context is INodeContext<DOM.Antlr.Parameter>) _context.NamespaceResolver.ProcessParameter(((INodeContext<DOM.Antlr.Parameter>) context).Node);

        }

        public override void ExitString_value_inline(MalinaParser.String_value_inlineContext context)
        {
            base.ExitString_value_inline(context);
            SendInterpolationAliasesToNameresolver();
        }

        private void SendInterpolationAliasesToNameresolver()
        {
            var parent = (IValueNode)_nodeStack.Peek();
            if (parent?.InterpolationAliases == null) return;
            foreach (var alias in parent.InterpolationAliases)
            {
                _context.NamespaceResolver.AddAlias(alias);
            }
        }

        public override void ExitString_value_ml(MalinaParser.String_value_mlContext context)
        {
            base.ExitString_value_ml(context);
            SendInterpolationAliasesToNameresolver();
        }

        protected override void EnterContext<T>(INodeContext<T> context, bool valueNode = false)
        {
            base.EnterContext(context, valueNode);
            var nodeContext = context as INodeContext<Alias>;
            if (nodeContext != null)
                _context.NamespaceResolver.AddAlias(nodeContext.Node);
        }

    }
}