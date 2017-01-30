#region license
// Copyright © 2016, 2017 Maxim Trushin (dba Syntactik, trushin@gmail.com, maxim.trushin@syntactik.com)
//
// This file is part of Malina.
// Malina is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// Malina is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Malina.  If not, see <http://www.gnu.org/licenses/>.
#endregion
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
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

                parser.RemoveErrorListeners();
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
