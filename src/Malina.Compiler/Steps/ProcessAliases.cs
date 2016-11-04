using System;
using System.IO;

namespace Malina.Compiler.Steps
{
    public class ProcessAliases : ICompilerStep    
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
            foreach (var input in _context.Parameters.Input)
            {
                try
                {
                    using (var reader = input.Open())
                        DoProcessAliases(input.Name, reader);
                }
                catch (Exception x)
                {
                    _context.Errors.Add(CompilerErrorFactory.InputError(input.Name, x));
                }
            }

        }

        private void DoProcessAliases(string name, TextReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
