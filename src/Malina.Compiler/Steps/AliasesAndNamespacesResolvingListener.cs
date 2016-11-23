using Antlr4.Runtime;
using Malina.DOM;
using Malina.DOM.Antlr;
using Malina.Parser;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using System;
using System.Linq;

namespace Malina.Compiler.Steps
{
    /// <summary>
    /// Creates Malina DOM structure (because it is inherited from MalinaParserListener).
    /// Populates list of AliasDefinitions in the CompilerContext.
    /// Resolves namespaces for documents
    /// </summary>
    public class AliasesAndNamespacesResolvingListener : MalinaParserListener
    {
        private CompilerContext _context;
        private string _fileName;

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
            _context.AliasDefinitions.Add(context.Node.Name, context.Node);
        }

        public override void ExitEveryRule([NotNull] ParserRuleContext context)
        {
            base.ExitEveryRule(context);

            if (context is INodeContext<DOM.Antlr.Element>) _context.NamespaceResolver.ProcessNsPrefix((context as INodeContext<DOM.Antlr.Element>).Node);
            else if (context is INodeContext<DOM.Antlr.Attribute>) _context.NamespaceResolver.ProcessNsPrefix((context as INodeContext<DOM.Antlr.Attribute>).Node);
            else if (context is INodeContext<DOM.Antlr.Alias>) _context.NamespaceResolver.ProcessAlias((context as INodeContext<DOM.Antlr.Alias>).Node);
            else if (context is INodeContext<DOM.Antlr.Parameter>) _context.NamespaceResolver.ProcessParameter((context as INodeContext<DOM.Antlr.Parameter>).Node);

        }
    }
}