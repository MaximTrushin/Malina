using Antlr4.Runtime;
using Malina.Parser;
using Antlr4.Runtime.Misc;
using Malina.DOM.Antlr;
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

            var elementContext = context as INodeContext<Element>;
            if (elementContext != null)
            {
                _context.NamespaceResolver.ProcessNsPrefix(elementContext.Node);
                return;
            }

            var attributeContext = context as INodeContext<Attribute>;
            if (attributeContext != null)
            {
                _context.NamespaceResolver.ProcessNsPrefix(attributeContext.Node);
                return;
            }

            var scopeContext = context as INodeContext<Scope>;
            if (scopeContext != null)
            {
                if(!string.IsNullOrEmpty(scopeContext.Node.NsPrefix))
                    _context.NamespaceResolver.ProcessNsPrefix(scopeContext.Node);
                return;
            }

            var aliasContext = context as INodeContext<Alias>;
            if (aliasContext != null)
            {
                _context.NamespaceResolver.ProcessAlias(aliasContext.Node);
                return;
            }

            var parameterContext = context as INodeContext<Parameter>;
            if (parameterContext != null)
            {
                _context.NamespaceResolver.ProcessParameter(parameterContext.Node);
                return;
            }
        }

        public override void ExitString_value_inline(MalinaParser.String_value_inlineContext context)
        {
            base.ExitString_value_inline(context);
            SendInterpolationAliasesAndParametersToNameresolver();
        }

        public override void ExitString_value_ml(MalinaParser.String_value_mlContext context)
        {
            base.ExitString_value_ml(context);
            SendInterpolationAliasesAndParametersToNameresolver();
        }

        private void SendInterpolationAliasesAndParametersToNameresolver()
        {
            var parent = (IValueNode)_nodeStack.Peek();
            if (parent?.InterpolationItems == null) return;
            foreach (var node in parent.InterpolationItems)
            {
                var alias = node as Alias;
                if (alias != null)
                {
                    _context.NamespaceResolver.AddAlias(alias);
                    continue;
                }

                var param = node as DOM.Parameter;
                if (param != null)
                {
                    _context.NamespaceResolver.ProcessParameter(param);
                    continue;
                }

            }
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