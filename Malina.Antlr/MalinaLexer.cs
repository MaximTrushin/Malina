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


        /// <summary>
        /// Problem is that IndentDedent and OsIndentDedent are not called for EOF in some cases.
        /// I had to add code to add NEWLINE if EOF is reached. This field is used to not add NEWLINE more than once. 
        /// </summary>
        private IToken _lastToken = null;

        private bool InWsaMode
        {
            get { return _wsaStack.Count > 0; }
        }

        public override void Reset()
        {
            InvalidTokens = new List<IToken>();
            _indents = new Stack<int>(new[] { 0 });
            _tokens = new Queue<IToken>();
            _wsaStack = new Stack<int>();            
            _lastToken = null;
            base.Reset();
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
            _lastToken = token;
        }

        public override IToken NextToken()
        {
            //Return previosly generated tokens first
            if (_tokens.Count > 0)
            {
                return _tokens.Dequeue();
            }
            
            Token = null;
            //Checking EOF and still mode = IN_VALUE
            //Scenario: Open value ends with EOF
            if (_input.La(1) == Eof && _mode == 1)
            {
                EmitIndentationToken(NEWLINE, CharIndex, CharIndex);
                PopMode();
            }
            //Checking if Dedents need to be generated in the EOF
            if (_input.La(1) == Eof && _indents.Peek() > 0)
            {
                if (InWsaMode)
                {
                    //If still in WSA mode in the EOF then clear wsa mode.
                    while (_wsaStack.Count > 0) _wsaStack.Pop();
                }

                if(_lastToken.Type != NEWLINE)
                    EmitIndentationToken(NEWLINE, CharIndex, CharIndex);

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
            if (_tokens.Count == 0) {
                return base.NextToken();
            }
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
            //Lexer reached EOF. 
            if (_input.La(1) == Eof)
            {
                //Adding trailing NEWLINE and DEDENTS if neeeded
                //Scenario: Any indented last node in the file.
                if (_indents.Count > 1)
                {
                    EmitIndentationToken(NEWLINE, CharIndex, CharIndex);                    
                    return;
                }
                else
                {
                    //Ignore indents in the end of file if there are no DEDENTS have to be reported. See comment above.
                    Skip();
                    return;
                }
            }

            //Lexer hasn't reached EOF
            var indent = CalcIndent();
            int prevIndent = _indents.Peek();
            if (indent == prevIndent)
            {
                //Emitting NEWLINE
                //Scenario: Any node in the end of line and not EOF.
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
                //Scenario: Any node in the end of line and not EOF.
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
                {
                    EmitIndentationToken(NEWLINE, CharIndex - indent - 1, CharIndex - indent - 1);
                }
                else
                {
                    //if value was ended with ==                                       
                    EmitExtraOSIndent(indent, _currentIndent + 1);//Emitting empty OS Indent to add EOL.

                    if (_input.La(1) == Eof)
                    {
                        EmitIndentationToken(NEWLINE, CharIndex - indent - 1, CharIndex - indent - 1);
                    }
                }
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

        private void DqIndentDedent()
        {
            var _currentIndent = _indents.Peek();
            var indent = CalcOsIndent();
            
            if (indent == _currentIndent)
            {
                //Checking if this is multiline DQS_VALUE_EOL
                ProcessDqIndentDedent(_tokenStartCharIndex, CharIndex - 1, _currentIndent);
                
                //Emitting NEWLINE 
                EmitIndentationToken(NEWLINE, CharIndex - indent - 1, CharIndex - indent - 1);
                PopMode(); PopMode();
            }
            else if (indent > _currentIndent)
            {
                //Checking if this is multiline DQS_VALUE_EOL
                var tokenGenerated = ProcessDqIndentDedent(_tokenStartCharIndex, CharIndex - 1, _currentIndent);

                if (indent > _currentIndent + 1)
                {
                    //Indent should be included in the Open String Value
                    EmitExtraDQSIndent(indent, _currentIndent);
                }
                else //if not ExtraOSIndent then SKIP
                    if(!tokenGenerated) Skip();
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
                PopMode(); PopMode();
            }
        }
        /// <summary>
        /// Generates additional NEWLINES and indents if DQS_VALUE_EOL consumes more then one line
        /// </summary>
        /// <param name="indentStart"></param>
        /// <param name="indentStop"></param>
        /// <param name="_currentIndent"></param>
        /// <returns></returns>
        private bool ProcessDqIndentDedent(int indentStart, int indentStop, int _currentIndent)
        {
            //checking if there is more than one line captured by the interval
            var lastNL = FindLastNewLine(indentStart, indentStop, this.InputStream);
            if (lastNL != -1)
                return GenerateEmptyLines(indentStart, lastNL, _currentIndent);
            return false;
        }

        private bool GenerateEmptyLines(int start, int stop, int _currentIndent)
        {
            var i = start;
            var s = InputStream.ToString();
            //var s1 = s.Substring(start, stop - start + 1);
            var s1 = s.Substring(start-2, stop - start + 10);
            var indent = 0;

            var tokenGenerated = false;


            //Skipping first /r/n
            if (InputStream.La(i - CharIndex) == '\r') i++;
            if (InputStream.La(i - CharIndex) == '\n') i++;
            var lineStart = i;

            while (i <= stop)
            {
                var c = InputStream.La(i - CharIndex);
                if (c != '\n' && c != '\r')
                    indent++;
                if (c == '\n' || c == '\r' || i == stop)
                {
                    if(indent > _currentIndent)
                    {
                        //Generate line with extra indent
                        EmitExtraDQSIndentToken(lineStart + _currentIndent + 1, i);
                        tokenGenerated = true;
                    }
                    else
                    {
                        //Generate just NEWLINE
                        EmitExtraDQSIndentToken(i, i-1);
                        tokenGenerated = true;
                    }

                    //Reseting indent counter
                    indent = 0;

                    //Skipping \r \n
                    if (c == '\r' && InputStream.La(i - CharIndex + 1) == '\n') i++;
                    //Saving new line start
                    lineStart = i + 1;
                }

                i++;
            }
            if(!tokenGenerated) EmitExtraDQSIndentToken(start, start - 1);
            return true;
        }

        public static int FindLastNewLine(int start, int stop, IIntStream input)
        {
            for (int i = stop - input.Index; i >= start - input.Index + 1; i--)
            {
                if (input.La(i) == '\r') return input.Index + i;
                if (input.La(i) == '\n') {
                    if (i > 0 && input.La(i - 1) != '\r') return input.Index + i;
                    else return input.Index + i - 1 >= start?input.Index + i - 1:-1;
                }
            }
            return -1;
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

        private void EmitExtraDQSIndent(int indent, int currentIndent)
        {            
            EmitIndentationToken(DQS_VALUE_EOL, InputStream.Index - indent + currentIndent + 1, InputStream.Index - 1);
        }
        private void EmitExtraDQSIndentToken(int start, int stop)
        {
                EmitIndentationToken(DQS_VALUE, start, stop);
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
