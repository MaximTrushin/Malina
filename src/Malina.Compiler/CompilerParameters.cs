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
        private readonly List<ICompilerInput> _input = new List<ICompilerInput>();
        private readonly XmlSchemaSet _xmlSchemaSet = new XmlSchemaSet();
        public string OutputDirectory { get; set; }

        public CompilerPipeline Pipeline { get; set; }


        public List<ICompilerInput> Input
        {
            get { return _input; }
        }

        public XmlSchemaSet XmlSchemaSet
        {
            get { return _xmlSchemaSet; }
        }

    }
}
