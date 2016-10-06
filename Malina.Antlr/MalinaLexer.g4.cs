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

            //Checking if Dedents need to be generated in the EOF
            if (_input.La(1) == Eof && _indents.Peek() > 0)
            {
                while (_indents.Peek() > 0)
                {
                    EmitToken(DEDENT, CharIndex, CharIndex);
                    _indents.Pop();
                }

                //Return one dedent if it was generated. Other dedents could be in _tokens
                if (Token != null)
                    return base.NextToken();
            }

            //Run regular path if there no extra tokens generated in queue
            if (_tokens.Count == 0)
                return base.NextToken();
            
            //Return generated tokens
            return _tokens.Dequeue();            
        }

        public override void Recover(LexerNoViableAltException e)
        {
            //Lexer recover strategy - ignore all till next space, tab or EOL
            while (_input.La(1) != -1 && _input.La(1) != ' ' && _input.La(1) != '\n' && _input.La(1) != '\r' && _input.La(1) != '\t')
            {
                Interpreter.Consume(_input);
            }
        }

        private void IndentDedent()
        {
            if (_input.La(1) == -1)
            {
                if (_indents.Count > 1)
                {
                    //We have to return Dedent here, otherwise NextToken will return EOF and stop. It will cause unreported DEDENTs.
                    Emit(new CommonToken(new Tuple<ITokenSource, ICharStream>(this, (this as ITokenSource).InputStream), DEDENT, Channel, CharIndex, CharIndex));
                    _indents.Pop();
                    return;
                }
                else
                {
                    //Ignore indents in the end of file if there no DEDENTS have to be reported. See comment above.
                    Skip();
                    return;
                }
            }

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
                    Emit(new CommonToken(new Tuple<ITokenSource, ICharStream>(this, (this as ITokenSource).InputStream), DEDENT, Channel, CharIndex - indent, CharIndex - 1));
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

        //Calculates Indent for Multiline Open String
        private int CalcOsIndent()
        {
            var i = -1;
            var osIndent = 0;
            int la = InputStream.La(i);
            while ( la != '\n' && la != -1 && la != '\r')
            {
                if (la != '=')
                    osIndent++;
                la = InputStream.La(--i);
            }
            //return -i - 1;
            return osIndent;
        }
        private void OsIndentDedent()
        {
            var _currentIndent = _indents.Peek();
            var indent = CalcOsIndent();
            if (indent == _currentIndent)
            {
                //Emitting NEWLINE
                EmitToken(NEWLINE, CharIndex - indent - 1, CharIndex - indent - 1);
                PopMode();
            }
            else if (indent > _currentIndent)
            {
                if (indent > _currentIndent + 1)
                {
                    EmitExtraOSIndent(indent, _currentIndent);
                }
                else //if not ExtraOSIndent then SKIP
                    Skip();
            }
            else
            {
                //Emitting 1 or more DEDENTS
                while (_indents.Count > 1 && _indents.Peek() > indent)
                {
                    //Emit(new CommonToken(new Tuple<ITokenSource, ICharStream>(this, (this as ITokenSource).InputStream), DEDENT, Channel,));
                    EmitToken(DEDENT, CharIndex - indent, CharIndex - 1);
                    _indents.Pop();
                }
                PopMode();
            }
        }

        private void EmitToken(int tokenType, int start, int stop)
        {
            Emit(new CommonToken(new Tuple<ITokenSource, ICharStream>(this, (this as ITokenSource).InputStream), tokenType, Channel, start, stop));
        }

        private void EmitExtraOSIndent(int indent, int currentIndent)
        {
            //var extraIndent = (InputStream as AntlrInputStream).GetText(new Antlr4.Runtime.Misc.Interval(InputStream.Index - indent, InputStream.Index));
            EmitToken(OPEN_VALUE_INDENT, InputStream.Index - indent + currentIndent + 1, InputStream.Index - 1);
        }
    }
}
