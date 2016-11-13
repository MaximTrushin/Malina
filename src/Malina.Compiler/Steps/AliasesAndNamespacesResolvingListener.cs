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
    /// Creates Malina DOM structure (because it is inherited fron MalinaParserListener).
    /// Populates list of AliasDefinitions in the CompilerContext.
    /// Resolves namespaces for documents
    /// </summary>
    public class AliasesAndNamespacesResolvingListener : MalinaParserListener
    {
        #region CLASS members
        private CompilerContext _context;
        private Module _module; //Current module
        private DOM.Document _document; //Current document
        private DOM.AliasDefinition _aliasDefinition; //Current Alias Definition


        public AliasesAndNamespacesResolvingListener(CompilerContext context): base(context.CompileUnit)
        {
            _context = context;
        }
        #endregion

        public override void EnterModule(MalinaParser.ModuleContext context)
        {
            base.EnterModule(context);
            _module = context.Node;
        }

        public override void EnterDocument_stmt([NotNull] MalinaParser.Document_stmtContext context)
        {
            base.EnterDocument_stmt(context);

            //Set document as context
            _document = context.Node;
            _aliasDefinition = null;
        }

        public override void EnterAlias_def_stmt([NotNull] MalinaParser.Alias_def_stmtContext context)
        {
            base.EnterAlias_def_stmt(context);

            //Set AliasDef as context
            _aliasDefinition = context.Node;
            _document = null;

        }

        public override void ExitAlias_def_stmt(MalinaParser.Alias_def_stmtContext context)
        {
            base.ExitAlias_def_stmt(context);
            _context.AliasDefinitions.Add(context.Node.Name, context.Node);
        }

        public override void ExitEveryRule([NotNull] ParserRuleContext context)
        {
            base.ExitEveryRule(context);
            if (context is INodeContext<DOM.Antlr.Element>) CheckNameSpacePrefix(context as INodeContext<DOM.Antlr.Element>);
            if (context is INodeContext<DOM.Antlr.Attribute>) CheckNameSpacePrefix(context as INodeContext<DOM.Antlr.Attribute>);

        }

        private void CheckNameSpacePrefix<T>(INodeContext<T> context) where T : Node
        {
            var node = context.Node as INsNode;

            if(node.NsPrefix != null && !node.NsPrefix.StartsWith("xml", StringComparison.OrdinalIgnoreCase) && !PrefixExists(node.NsPrefix))
            {
                _context.Errors.Add(CompilerErrorFactory.NsPrefixNotDefined(context.Node, _module.FileName));
            }
        }

        private bool PrefixExists(string nsPrefix)
        {
            if (_document != null && _document.Namespaces.ContainsKey(nsPrefix))
                 return true;

            if (_aliasDefinition != null && _aliasDefinition.Namespaces.Any(n => n.Name == nsPrefix))
                return true;

            if (_module.Namespaces.Any(n => n.Name == nsPrefix))
                return true;

            return false;
        }
    }
}