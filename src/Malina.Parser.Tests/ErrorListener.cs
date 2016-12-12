using Antlr4.Runtime;
using System.Collections.Generic;
using Malina.Parser;

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
                    Errors.Add(me);
                }
            }
            HasErrors = true;
        }
    }
}
