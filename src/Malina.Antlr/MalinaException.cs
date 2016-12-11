using Antlr4.Runtime;

namespace Malina.Parser
{
    public class MalinaException: RecognitionException
    {
        public MalinaException(Lexer lexer, ICharStream input, MalinaError error): base(lexer, input)
        {
            Error = error;
        }

        public MalinaError Error { get; }
    }
}
