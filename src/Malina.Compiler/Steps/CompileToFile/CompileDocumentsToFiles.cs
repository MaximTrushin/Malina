using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Malina.DOM;
using Malina.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Malina.Generator;
using Newtonsoft.Json;

namespace Malina.Compiler.Steps
{
    class CompileDocumentsToFiles : ICompilerStep
    {
        CompilerContext _context;
        public void Dispose()
        {
            _context = null;
        }

        public void Initialize(CompilerContext context)
        {
            _context = context;
        }

        public void Run()
        {
            try
            {
                foreach (var module in _context.CompileUnit.Modules)
                {
                    if (_context.Errors.Count > 0) break;
                    DoCompileDocumentsToFile(module, _context);
                }
            }
            catch (Exception ex)
            {
                _context.Errors.Add(CompilerErrorFactory.FatalError(ex));
            }
        }

        private void DoCompileDocumentsToFile(Module module, CompilerContext context)
        {
            try
            {
                Directory.CreateDirectory(context.Parameters.OutputDirectory);

                MalinaDepthFirstVisitor visitor;
                if (module.FileName.EndsWith(".mlx"))
                    visitor = new CompilingXmlToFileVisitor(context);
                else visitor = new JsonGenerator(JsonFileWriterDelegate);

                visitor.OnModule(module);
            }
            catch (Exception ex)
            {
                _context.Errors.Add(CompilerErrorFactory.FatalError(ex));
            }
        }

        private JsonWriter JsonFileWriterDelegate(string documentName)
        {
            var fileName = Path.Combine(_context.Parameters.OutputDirectory, documentName + ".json");
            if (File.Exists(fileName)) File.Delete(fileName);
            TextWriter writer = new StreamWriter(fileName);
            return new JsonTextWriter(writer) {Formatting = Formatting.Indented}; //todo: add cmd line argument to change formatting.+
        }
    }
}
