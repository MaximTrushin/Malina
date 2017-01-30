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
using Malina.DOM;
using System;
using System.Collections.Generic;

namespace Malina.Compiler
{
    public class CompilerContext
    {
        public CompileUnit CompileUnit { get; private set; }
        public CompilerParameters Parameters { get; private set; }

        public NamespaceResolver NamespaceResolver { get; private set; }

        public SortedSet<CompilerError> Errors { get; }

        public CompilerContext(CompilerParameters parameters, CompileUnit compileUnit)
        {
            Parameters = parameters;
            CompileUnit = compileUnit;
            Errors = new SortedSet<CompilerError>();            
            NamespaceResolver = new NamespaceResolver(this);
        }

        public void AddError(CompilerError error)
        {
            if (Errors.Count >= 1000)
                throw new ApplicationException("Number of compiler error exceeds 1000.");

            Errors.Add(error);
        }


    }
}
