using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Malina.Parser
{
    public class MalinaErrorStrategy : DefaultErrorStrategy
    {

        public override void Recover(Antlr4.Runtime.Parser recognizer, RecognitionException e)
        {

            int tokenType = ((ITokenStream)recognizer.InputStream).La(1);
            int indents = 1;
            while (tokenType != TokenConstants.Eof && indents > 0)
            {
                recognizer.Consume();
                tokenType = ((ITokenStream)recognizer.InputStream).La(1);
                if (tokenType == MalinaLexer.INDENT)
                    indents++;
                if (tokenType == MalinaLexer.DEDENT)
                    indents--;
            }

        }

        protected override void ConsumeUntil(Antlr4.Runtime.Parser recognizer, IntervalSet set)
        {
            int tokenType = ((ITokenStream)recognizer.InputStream).La(1);
            int indents = 1;
            while (tokenType != TokenConstants.Eof && indents > 0)
            {
                recognizer.Consume();
                tokenType = ((ITokenStream)recognizer.InputStream).La(1);
                if (tokenType == MalinaLexer.INDENT)
                    indents++;
                if (tokenType == MalinaLexer.DEDENT)
                    indents--;
            }
        }

    }
}
