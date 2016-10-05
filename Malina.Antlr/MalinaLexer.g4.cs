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
        private Stack<int> _indents = new Stack<int>();
        private void IndentDedent()
        {
            var i = -1;
            while (InputStream.La(i) != '\n' && InputStream.La(i) != -1 && InputStream.La(i) != '\r') i--;
            var indent = -i-1;



        }
    }
}
