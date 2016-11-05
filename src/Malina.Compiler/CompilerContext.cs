using Malina.DOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malina.Compiler
{
    public class CompilerContext
    {
        public CompileUnit CompileUnit { get; private set; }
        public CompilerParameters Parameters { get; private set; }

        public List<CompilerError> Errors { get; private set; }

        public CompilerContext(CompilerParameters parameters, CompileUnit compileUnit)
        {
            Parameters = parameters;
            CompileUnit = compileUnit;
            Errors = new List<Compiler.CompilerError>();
        }


    }
}
