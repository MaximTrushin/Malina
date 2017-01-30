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

using System;

namespace Malina.Parser
{
    [Serializable]
    public enum MalinaErrorCode
    {
        [Message("")] NoError = 0,

        //Lexer Errors
        [Message("Missing closing Double Quote.")] ClosingDqMissing,
        [Message("Missing closing Single Quote.")] ClosingSqMissing,
        [Message("Incorrect usage of colon.")] IncorrectColon,
        [Message("Unexpected symbol.")] LexerNoViableAltException,

        //Parser Errors
        [Message("NoViableAltParserException")] NoViableAltParserException,
        
    }

    //[Serializable]
    //public class MalinaError
    //{
    //    private MalinaErrorCode _code;
    //    private SourceLocation _start;
    //    private SourceLocation _stop;

    //    public SourceLocation Start
    //    {
    //        get
    //        {
    //            return _start;
    //        }
    //        set { _start = value; }
    //    }

    //    public MalinaErrorCode Code
    //    {
    //        get
    //        {
    //            return _code;
    //        }
    //        set { _code = value; }
    //    }

    //    public SourceLocation Stop
    //    {
    //        get
    //        {
    //            return _stop;
    //        }
    //        set { _stop = value; }
    //    }

    //    //public RecognitionException Exception
    //    //{
    //    //    get
    //    //    {
    //    //        return _exception;
    //    //    }

    //    //    set
    //    //    {
    //    //        _exception = value;
    //    //    }
    //    //}

    //    public MalinaError() { }
    //    public MalinaError(MalinaErrorCode code, SourceLocation start, SourceLocation stop)
    //    {
    //        _code = code;
    //        _start = start;
    //        _stop = stop;
    //    }
    //}
}


