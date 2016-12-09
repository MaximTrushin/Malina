using Malina.DOM;
using System;
using System.IO;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

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
                    visitor = new XmlGenerator(XmlFileWriterDelegate, context);
                else visitor = new JsonGenerator(JsonFileWriterDelegate);

                visitor.OnModule(module);
            }
            catch (Exception ex)
            {
                _context.Errors.Add(CompilerErrorFactory.FatalError(ex));
            }
        }

        private XmlWriter XmlFileWriterDelegate(string documentName)
        {
            var fileName = Path.Combine(_context.Parameters.OutputDirectory + documentName + ".xml");
            return XmlWriter.Create(
                new XmlTextWriter(fileName, Encoding.UTF8) {Formatting = System.Xml.Formatting.Indented, Namespaces = true},
                new XmlWriterSettings {ConformanceLevel = ConformanceLevel.Document});

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
