﻿#region license
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
using Malina.Compiler.Steps;

namespace Malina.Compiler.Pipelines
{
    public class CompileToFiles: CompilerPipeline
    {
        public CompileToFiles()
        {
            Steps.Add(new ProcessAliasesAndNamespaces());
            Steps.Add(new ValidateDocuments());
            Steps.Add(new CompileDocumentsToFiles());
        }
    }
}
