using Antlr4.Runtime;
using System.Collections.Generic;
using Malina.Parser;

namespace Malina.Parser.Tests
{
    public class ErrorListener<T> : IAntlrErrorListener<T>
    {
        public List<MalinaError> Errors { get; } = new List<MalinaError>();

        public bool HasErrors { get; private set; }

        public void SyntaxError(IRecognizer recognizer, T offendingSymbol, int line, int charPositionInLine, string msg,
            RecognitionException e)
        {
            if (e is MalinaException) Errors.Add(((MalinaException) e).Error);
            else
            {
                var me = new MalinaError(MalinaErrorCode.NoViableAltParserException, null, null);
                Errors.Add(me);
            }
            HasErrors = true;
        }
    }
}
