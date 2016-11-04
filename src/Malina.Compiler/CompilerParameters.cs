using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malina.Compiler
{
    public class CompilerParameters
    {
        private readonly List<ICompilerInput> _input = new List<ICompilerInput>();
        public string OutputDirectory { get; set; }

        public CompilerPipeline Pipeline { get; set; }


        public List<ICompilerInput> Input
        {
            get { return _input; }
        }


    }
}
