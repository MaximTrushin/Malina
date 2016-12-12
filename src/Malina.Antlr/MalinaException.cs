using System.Text;
using Antlr4.Runtime;
using Malina.DOM;

namespace Malina.Parser
{
    public class MalinaException: RecognitionException
    {
        public SourceLocation Start { get; set; }

        public MalinaErrorCode Code { get; set; }

        public SourceLocation Stop { get; set; }

        public MalinaException(Lexer lexer, ICharStream input): base(lexer, input)
        {
        }

        public MalinaException(IRecognizer recognizer, IIntStream input, ParserRuleContext ctx): base(recognizer, input, ctx)
        {
            
        }

        public MalinaException(string message, IRecognizer recognizer, IIntStream input, ParserRuleContext ctx):base(message, recognizer, input,ctx)
        {
            
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Code={Code}, Start={Start}, Stop={Stop}");
            return sb.ToString();
        }
    }
}
