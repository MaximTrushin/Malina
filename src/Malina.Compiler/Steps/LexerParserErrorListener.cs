using Antlr4.Runtime;
using System.Collections.Generic;
using Malina.Parser;

namespace Malina.Compiler
{
    public class LexerParserErrorListener<T> : IAntlrErrorListener<T>
    {
        private CompilerContext _context;
        private List<MalinaError> _errors = new List<MalinaError>();
        private string _fileName;

        public LexerParserErrorListener(CompilerContext context, string fileName)
        {
            _context = context;
            _fileName = fileName;
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
                if(recognizer is MalinaParser)
                {
                    _context.Errors.Add(CompilerErrorFactory.ParserError(e, msg, _fileName, line, charPositionInLine));
                }
                else if(recognizer is MalinaLexer)
                {
                    _context.Errors.Add(CompilerErrorFactory.LexerError(e, msg, _fileName, line, charPositionInLine));
                }
                else
                {
                    _context.Errors.Add(CompilerErrorFactory.FatalError(e, msg));
                }
            }
        }
    }
}
