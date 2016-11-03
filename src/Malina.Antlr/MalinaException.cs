using Antlr4.Runtime;

namespace Malina.Parser
{
    public class MalinaException: RecognitionException
    {
        private MalinaError _error;
        public MalinaException(Lexer lexer, ICharStream input, MalinaError error): base(lexer, input)
        {
            _error = error;
        }

        public MalinaError Error
        {
            get
            {
                return _error;
            }
        }
    }
}
