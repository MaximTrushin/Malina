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
using System.Collections.Generic;
using Malina.Parser;

namespace Malina.Compiler
{
    public class LexerParserErrorListener<T> : IAntlrErrorListener<T>
    {
        private readonly CompilerContext _context;
        private readonly string _fileName;

        public LexerParserErrorListener(CompilerContext context, string fileName)
        {
            _context = context;
            _fileName = fileName;
        }

        public List<MalinaException> Errors { get; } = new List<MalinaException>();


        public void SyntaxError(IRecognizer recognizer, T offendingSymbol, int line, int charPositionInLine, string msg,
            RecognitionException e)
        {

                if(recognizer is MalinaParser)
                {
                    if (offendingSymbol != null)
                    {
                        var tokenName =
                            ((MalinaParser) recognizer).Vocabulary.GetDisplayName((offendingSymbol as CommonToken).Type);
                        _context.AddError(CompilerErrorFactory.ParserError(e,
                            $"Unexpected token {tokenName}<'{(offendingSymbol as CommonToken).Text}'>",
                            _fileName, (offendingSymbol as CommonToken).Line,
                            (offendingSymbol as CommonToken).Column + 1));
                    }
                    else
                    {
                        _context.AddError(CompilerErrorFactory.ParserError(e, msg, _fileName, line,
                            charPositionInLine + 1));
                    }
                }
                else if(recognizer is MalinaLexer)
                {
                    _context.AddError(CompilerErrorFactory.LexerError(e, msg, _fileName, line, charPositionInLine));
                }
                else
                {
                    _context.AddError(CompilerErrorFactory.FatalError(e, msg));
                }

        }
    }
}
