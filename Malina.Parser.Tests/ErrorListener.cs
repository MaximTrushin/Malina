using Antlr4.Runtime;
using System.Collections.Generic;
using Malina.Parser;

namespace Malina.Parser.Tests
{
    public class ErrorListener<T> : IAntlrErrorListener<T>
    {
        private List<MalinaError> _errors = new List<MalinaError>();

        public List<MalinaError> Errors 
        {
            get
            {
                return _errors;
            }
       }

        public bool HasErrors { get; private set; }

        public void SyntaxError(IRecognizer recognizer, T offendingSymbol, int line, int charPositionInLine, string msg,
            RecognitionException e)
        {
            if (e is MalinaException) _errors.Add((e as MalinaException).Error);
            else
            {
                var me = new MalinaError(MalinaErrorCode.NoViableAltParserException, null, null);
                //_errors.Add(me);
            }
            HasErrors = true;
        }
    }
}
