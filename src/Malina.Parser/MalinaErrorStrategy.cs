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
