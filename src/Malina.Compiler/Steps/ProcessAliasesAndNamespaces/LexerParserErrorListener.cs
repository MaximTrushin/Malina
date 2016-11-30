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
                    if (e is NoViableAltException)
                    {
                        var ex = e as NoViableAltException;
                        var tokenName = (recognizer as MalinaParser).Vocabulary.GetDisplayName((offendingSymbol as CommonToken).Type);
                        _context.Errors.Add(CompilerErrorFactory.ParserError(e, string.Format("Unexpected token {0}<'{1}'>", tokenName, (offendingSymbol as CommonToken).Text), 
                            _fileName, (offendingSymbol as CommonToken).Line, (offendingSymbol as CommonToken).Column + 1));
                    }
                    else
                    {
                        _context.Errors.Add(CompilerErrorFactory.ParserError(e, msg, _fileName, line, charPositionInLine + 1));
                    }
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
