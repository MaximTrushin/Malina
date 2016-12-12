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
            if (e is MalinaException) Errors.Add((MalinaException) e);
            else
            {
                if(recognizer is MalinaParser)
                {
                    if (offendingSymbol != null)
                    {
                        var tokenName =
                            (recognizer as MalinaParser).Vocabulary.GetDisplayName((offendingSymbol as CommonToken).Type);
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
}
