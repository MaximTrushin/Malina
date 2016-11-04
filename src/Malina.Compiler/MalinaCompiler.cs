using Malina.DOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malina.Compiler
{
    public class MalinaCompiler
    {
        public CompilerParameters Parameters { get; private set; }

        public MalinaCompiler()
        {
            Parameters = new CompilerParameters();
        }
        public MalinaCompiler(CompilerParameters parameters)
        {
            Parameters = parameters;
        }

        public CompilerContext Run()
        {
            var compileUnit = new CompileUnit();
            var context = new CompilerContext(Parameters, compileUnit);
            Parameters.Pipeline.Run(context);
            return context;
        }
    }
}
