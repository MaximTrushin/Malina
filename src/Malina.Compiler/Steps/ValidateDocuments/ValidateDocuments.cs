using System;
using Malina.DOM;

namespace Malina.Compiler.Steps
{
    class ValidateDocuments: ICompilerStep
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
                    DoValidateDocuments(module, _context);
                }
            }
            catch (Exception ex)
            {
                _context.Errors.Add(CompilerErrorFactory.FatalError(ex));
            }
        }

        private void DoValidateDocuments(Module module, CompilerContext context)
        {
            try
            {
                MalinaDepthFirstVisitor visitor = new ValidatingDocumentsVisitor(context);

                visitor.OnModule(module);
            }
            catch (Exception ex)
            {
                _context.Errors.Add(CompilerErrorFactory.FatalError(ex));
            }
        }
    }
}
