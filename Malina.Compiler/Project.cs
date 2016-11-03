using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malina.Compiler
{
    public class Project
    {
        public Dictionary<string, MalinaCompileModule> Modules { get; private set; }

        public Project()
        {
            Modules = new Dictionary<string, MalinaCompileModule>();
        }
    }
}
