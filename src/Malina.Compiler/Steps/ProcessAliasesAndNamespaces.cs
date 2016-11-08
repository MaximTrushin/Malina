using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Malina.DOM;
using Malina.Parser;
using System;
using System.IO;

namespace Malina.Compiler.Steps
{
    public class ProcessAliasesAndNamespaces : ICompilerStep    
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
                        DoProcessAliasesAndNamespaces(input.Name, reader);
                }
                catch (Exception ex)
                {
                    _context.Errors.Add(CompilerErrorFactory.InputError(input.Name, ex));
                }
            }

        }

        private void DoProcessAliasesAndNamespaces(string name, TextReader reader)
        {
            var lexer = new MalinaLexer(new AntlrInputStream(reader));

            lexer.RemoveErrorListeners();            
            lexer.AddErrorListener(new LexerParserErrorListener<int>(_context));

            var parser = new MalinaParser(new CommonTokenStream(lexer));
            parser.Interpreter.PredictionMode = PredictionMode.Sll;
            var malinaListener = new AliasesAndNamespacesResolvingListener(_context);
           
            parser.AddErrorListener(new LexerParserErrorListener<IToken>(_context));
            parser.AddParseListener(malinaListener);
            parser.module();


        }
    }
}
