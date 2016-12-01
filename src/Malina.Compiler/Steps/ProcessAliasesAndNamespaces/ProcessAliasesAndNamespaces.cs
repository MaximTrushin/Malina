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
                        _context.AddError(CompilerErrorFactory.InputError(input.Name, ex));
                    }
                }
                _context.NamespaceResolver.ResolveAliasesAndDoChecks();
            }
            catch (Exception ex)
            {
                _context.Errors.Add(CompilerErrorFactory.FatalError(ex));
            }
        }

        private void DoProcessAliasesAndNamespaces(string fileName, TextReader reader)
        {
            try
            {
                var lexer = new MalinaLexer(new AntlrInputStream(reader));

                lexer.RemoveErrorListeners();
                lexer.AddErrorListener(new LexerParserErrorListener<int>(_context, fileName));

                var parser = MalinaParser.Create(new CommonTokenStream(lexer));
                parser.Interpreter.PredictionMode = PredictionMode.Sll;

                var resolvingListener = new AliasesAndNamespacesResolvingListener(_context, fileName);

                parser.AddErrorListener(new LexerParserErrorListener<IToken>(_context, fileName));
                parser.AddParseListener(resolvingListener);
                parser.module();

            }
            catch(Exception ex)
            {
                _context.AddError(CompilerErrorFactory.FatalError(ex));
            }
        }
    }
}
