using Malina.Compiler.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malina.Compiler.Pipelines
{
    public class CompileToFiles: CompilerPipeline
    {
        public CompileToFiles()
        {
            Steps.Add(new ProcessAliases());
        }
    }
}
