using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Malina.Compiler
{
    public class CompilerParameters
    {
        public string OutputDirectory { get; set; }

        public CompilerPipeline Pipeline { get; set; }


        public List<ICompilerInput> Input { get; } = new List<ICompilerInput>();

        public XmlSchemaSet XmlSchemaSet { get; } = new XmlSchemaSet();
    }
}
