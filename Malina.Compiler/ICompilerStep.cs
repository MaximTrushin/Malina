using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malina.Compiler
{
    public interface ICompilerStep
    {
        void Run();
        void Initialize(CompilerContext context);
    }
}
