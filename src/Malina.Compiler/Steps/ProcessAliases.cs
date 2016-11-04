using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malina.Compiler.Steps
{
    public class ProcessAliases : ICompilerStep    
    {
        CompilerContext _context;
        public void Initialize(CompilerContext context)
        {
            _context = context;                
        }

        public void Run()
        {
            
        }
    }
}
