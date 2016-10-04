using Antlr4.Runtime;

namespace Malina.Parser.Tests
{
    public class ErrorListener<T> : IAntlrErrorListener<T>
    {
        public bool HasErrors { get; private set; }

        public void SyntaxError(IRecognizer recognizer, T offendingSymbol, int line, int charPositionInLine, string msg,
            RecognitionException e)
        {
            HasErrors = true;
        }
    }
}
