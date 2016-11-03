using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malina.Compiler
{
    public class CompilerParameters
    {
        public readonly List<string> Files = new List<string>();
        public string OutputDirectory { get; set; }

        public CompilerPipeline Pipeline { get; set; }

    }
}
