using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malina.Parser
{
    partial class MalinaLexer
    {
        public List<IToken> InvalidTokens = new List<IToken>();
        private Stack<int> _indents = new Stack<int>(new[] { 0 });
        private Queue<IToken> _tokens = new Queue<IToken>();
        private void IndentDedent()
        {

            var indent = CalcIndent();
            int prevIndent = _indents.Peek();
            if (indent == prevIndent)
            {
                //Emitting NEWLINE
                Emit(new CommonToken(new Tuple<ITokenSource, ICharStream>(this, (this as ITokenSource).InputStream), NEWLINE, Channel, CharIndex - indent - 1, CharIndex - indent - 1));
            }
            else if (indent > prevIndent)
            {
                //Emitting INDENT
                _indents.Push(indent);
                Emit(new CommonToken(new Tuple<ITokenSource, ICharStream>(this, (this as ITokenSource).InputStream), INDENT, Channel, CharIndex - indent, CharIndex - 1));
            }
            else
            {
                //Emitting 1 or more DEDENTS
                while (_indents.Count > 1 && _indents.Peek() > indent)
                {
                    Emit(new CommonToken(new Tuple<ITokenSource, ICharStream>(this, (this as ITokenSource).InputStream), DEDENT, Channel, CharIndex - indent, CharIndex));
                    _indents.Pop();
                }
            }

        }

        private int CalcIndent()
        {
            var i = -1;
            while (InputStream.La(i) != '\n' && InputStream.La(i) != -1 && InputStream.La(i) != '\r') i--;
            return -i - 1;
        }


        public override void Emit(IToken token)
        {
            if (_token == null)
            {
                base.Emit(token);
            }
            else
            {
                _tokens.Enqueue(token);
            }            
        }

        public override IToken NextToken()
        {
            Token = null;

            if (_tokens.Count > 0)
                return _tokens.Dequeue();
            return base.NextToken();
        }
    }
}
