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
using Malina.DOM;

namespace Malina.Parser.Tests
{
    public class ErrorListener<T> : IAntlrErrorListener<T>
    {
        public List<MalinaException> Errors { get; } = new List<MalinaException>();

        public bool HasErrors { get; private set; }

        public void SyntaxError(IRecognizer recognizer, T offendingSymbol, int line, int charPositionInLine, string msg,
            RecognitionException e)
        {
            if (e is MalinaException) Errors.Add((MalinaException) e);
            else
            {
                if (recognizer is MalinaParser)
                {
                    var me = new MalinaException(msg, (MalinaParser)recognizer, ((MalinaParser)recognizer).InputStream, ((MalinaParser)recognizer).Context)
                    {
                        Code = MalinaErrorCode.NoViableAltParserException,
                        Start = new DOM.SourceLocation(line, charPositionInLine + 1, -1),
                        Stop = new DOM.SourceLocation(line, charPositionInLine + 1, -1)
                    };
                    Errors.Add(me);
                }
                else if (recognizer is MalinaLexer)
                {
                    var me = new MalinaException((Lexer)recognizer, ((ITokenSource)recognizer).InputStream);
                    me.Start = new SourceLocation(line, charPositionInLine+1, recognizer.InputStream.Index);
                    me.Start = new SourceLocation(line, charPositionInLine + 1, recognizer.InputStream.Index);
                    me.Code = MalinaErrorCode.LexerNoViableAltException;
 
                    Errors.Add(me);
                }
            }
            HasErrors = true;
        }
    }
}
