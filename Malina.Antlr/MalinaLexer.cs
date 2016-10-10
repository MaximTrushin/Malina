using Antlr4.Runtime;
using System;
using System.Collections.Generic;

namespace Malina.Parser
{
    partial class MalinaLexer
    {
        public List<IToken> InvalidTokens = new List<IToken>();
        private Stack<int> _indents = new Stack<int>(new[] { 0 });
        private Queue<IToken> _tokens = new Queue<IToken>();
        private Stack<int> _wsaStack = new Stack<int>();

        private bool InWsaMode
        {
            get { return _wsaStack.Count > 0; }
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

            //Checking if Dedents need to be generated in the EOF
            if (_input.La(1) == Eof && _indents.Peek() > 0)
            {
                if (InWsaMode)
                {
                    //If still in WSA mode in the EOF then clear wsa mode.
                    while (_wsaStack.Count > 0) _wsaStack.Pop();
                }

                while (_indents.Peek() > 0)
                {
                    EmitIndentationToken(DEDENT, CharIndex, CharIndex);
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
                //Lexer reached EOF. Adding trailing NEWLINE and DEDENTS if neeeded
                if (_indents.Count > 1)
                {
                    EmitIndentationToken(NEWLINE, CharIndex, CharIndex);
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
                if (_tokenStartCharIndex > 0) //Ignore New Line starting in BOF
                {
                    EmitIndentationToken(NEWLINE, CharIndex - indent - 1, CharIndex - indent - 1);
                }
                else Skip();
            }
            else if (indent > prevIndent)
            {
                //Emitting INDENT
                _indents.Push(indent);
                EmitIndentationToken(INDENT, CharIndex - indent, CharIndex - 1);
            }
            else
            {
                //Adding 1 NEWLINE before DEDENTS
                if (_indents.Count > 1 && _indents.Peek() > indent)
                {
                    EmitIndentationToken(NEWLINE, CharIndex - indent - 1, CharIndex - indent - 1);
                }
                //Emitting 1 or more DEDENTS
                while (_indents.Count > 1 && _indents.Peek() > indent)
                {
                    EmitIndentationToken(DEDENT, CharIndex - indent, CharIndex - 1);
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
            while (la != '\n' && la != -1 && la != '\r')
            {
                if (la != '=')
                    osIndent++;
                la = InputStream.La(--i);
            }
            //return -i - 1;
            return osIndent;
        }

        //Open String Indents/Dedents processing
        private void OsIndentDedent()
        {
            var _currentIndent = _indents.Peek();
            var indent = CalcOsIndent();
            if (indent == _currentIndent)
            {
                //Emitting NEWLINE if OS is not ended by ==
                if (_input.La(-1) != '=')
                    EmitIndentationToken(NEWLINE, CharIndex - indent - 1, CharIndex - indent - 1);
                else
                    Skip();
                PopMode();
            }
            else if (indent > _currentIndent)
            {
                if (indent > _currentIndent + 1)
                {
                    //Indent should be included in the Open String Value
                    EmitExtraOSIndent(indent, _currentIndent);
                }
                else //if not ExtraOSIndent then SKIP
                    Skip();
            }
            else
            {
                //Adding 1 NEWLINE before DEDENTS
                if (_indents.Count > 1 && _indents.Peek() > indent)
                {
                    EmitIndentationToken(NEWLINE, CharIndex - indent - 1, CharIndex - indent - 1);
                }
                //Emitting 1 or more DEDENTS
                while (_indents.Count > 1 && _indents.Peek() > indent)
                {
                    EmitIndentationToken(DEDENT, CharIndex - indent, CharIndex - 1);
                    _indents.Pop();
                }
                PopMode();
            }
        }

        private void EmitIndentationToken(int tokenType, int start, int stop)
        {
            if (InWsaMode)
                Skip();
            else
                Emit(new CommonToken(new Tuple<ITokenSource, ICharStream>(this, (this as ITokenSource).InputStream), tokenType, Channel, start, stop));
        }

        private void Emit(int tokenType)
        {
            Emit(new CommonToken(new Tuple<ITokenSource, ICharStream>(this, (this as ITokenSource).InputStream), tokenType, Channel, _tokenStartCharIndex, InputStream.Index - 1));
        }


        private void EmitExtraOSIndent(int indent, int currentIndent)
        {
            if (_input.La(-1) == '=') currentIndent = currentIndent - 2; // Add 2 more symbols if Open String is ended by dedented ==
            EmitIndentationToken(OPEN_VALUE_INDENT, InputStream.Index - indent + currentIndent + 1, InputStream.Index - 1);
        }

        private void EnterWsa()
        {
            _wsaStack.Push(_tokenStartCharIndex);
        }

        private void ExitWsa()
        {
            if (_wsaStack.Count > 0)
                _wsaStack.Pop();
        }
    }
}
