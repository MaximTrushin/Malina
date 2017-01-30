#region license
// Copyright © 2016, 2017 Maxim Trushin (dba Syntactik, trushin@gmail.com, maxim.trushin@syntactik.com)
//
// This file is part of Malina.
// Malina is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// Malina is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Malina.  If not, see <http://www.gnu.org/licenses/>.
#endregion

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
