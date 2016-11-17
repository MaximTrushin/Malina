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
            try
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
                _context.NamespaceResolver.ResolveAliasesInDocuments();
            }
            catch (Exception ex)
            {
                _context.Errors.Add(CompilerErrorFactory.FatalError(ex));
            }
        }

        private void DoProcessAliasesAndNamespaces(string name, TextReader reader)
        {
            try
            {
                var lexer = new MalinaLexer(new AntlrInputStream(reader));

                lexer.RemoveErrorListeners();
                lexer.AddErrorListener(new LexerParserErrorListener<int>(_context, name));

                var parser = new MalinaParser(new CommonTokenStream(lexer));
                parser.Interpreter.PredictionMode = PredictionMode.Sll;

                var resolvingListener = new AliasesAndNamespacesResolvingListener(_context);

                parser.AddErrorListener(new LexerParserErrorListener<IToken>(_context, name));
                parser.AddParseListener(resolvingListener);
                parser.module();

            }
            catch(Exception ex)
            {
                _context.Errors.Add(CompilerErrorFactory.FatalError(ex));
            }
        }
    }
}
