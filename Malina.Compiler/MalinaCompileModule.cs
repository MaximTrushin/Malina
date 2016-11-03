using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malina.Compiler
{
    public class MalinaCompileModule
    {
        private Project _project;

        public string FileName { get; set; }

        public MalinaCompileModule(Project project, string fileName, string outputPath = null)
        {
            _project = project;
            FileName = fileName;
        }
    }
}
