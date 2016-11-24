using Malina.DOM;
using System.Collections.Generic;

namespace Malina.Compiler
{
    public class CompilerContext
    {
        public CompileUnit CompileUnit { get; private set; }
        public CompilerParameters Parameters { get; private set; }

        public NamespaceResolver NamespaceResolver { get; private set; }

        public SortedSet<CompilerError> Errors { get; private set; }

        public CompilerContext(CompilerParameters parameters, CompileUnit compileUnit)
        {
            Parameters = parameters;
            CompileUnit = compileUnit;
            Errors = new SortedSet<CompilerError>();            
            NamespaceResolver = new NamespaceResolver(this);
        }


    }
}
