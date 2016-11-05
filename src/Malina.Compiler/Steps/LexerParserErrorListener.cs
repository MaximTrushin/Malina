using Antlr4.Runtime;
using System.Collections.Generic;
using Malina.Parser;

namespace Malina.Compiler
{
    public class LexerParserErrorListener<T> : IAntlrErrorListener<T>
    {
        private CompilerContext _context;
        private List<MalinaError> _errors = new List<MalinaError>();

        public LexerParserErrorListener(CompilerContext _context)
        {
            this._context = _context;
        }

        public List<MalinaError> Errors 
        {
            get
            {
                return _errors;
            }
       }


        public void SyntaxError(IRecognizer recognizer, T offendingSymbol, int line, int charPositionInLine, string msg,
            RecognitionException e)
        {
            if (e is MalinaException) _errors.Add((e as MalinaException).Error);
            else
            {
                var me = new MalinaError(MalinaErrorCode.NoViableAltParserException, null, null);
                _errors.Add(me);
            }
        }
    }
}
