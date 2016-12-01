using Malina.DOM;
using System;
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

        public void AddError(CompilerError error)
        {
            if (Errors.Count >= 1000)
                throw new ApplicationException("Number of compiler error exceeds 1000.");

            Errors.Add(error);
        }


    }
}
