using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malina.Compiler
{
    public class CompilerContext
    {
        private CompileUnit compileUnit;
        private CompilerParameters parameters;

        public CompilerContext(CompilerParameters parameters, CompileUnit compileUnit)
        {
            this.parameters = parameters;
            this.compileUnit = compileUnit;
        }
    }
}
