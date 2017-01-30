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
using System.Collections.Generic;
using Alias = Malina.DOM.Antlr.Alias;

namespace Malina.Compiler
{
    /// <summary>
    /// Collects a list of used namespaces and aliases in the ModuleMember (Document or AliasDef)
    /// </summary>
    public class NsInfo
    {
        public ModuleMember ModuleMember { get; private set; }
        public bool AliasesResolved { get; internal set; }
        private List<Namespace> _namespaces;
        private List<Alias> _aliases;

        public List<Namespace> Namespaces
        {
            get
            {
                return _namespaces ?? (_namespaces = new List<Namespace>());
            }

            set
            {
                _namespaces = value;
            }
        }

        public List<Alias> Aliases
        {
            get
            {

                return _aliases ?? (_aliases = new List<Alias>());
            }

            set
            {
                _aliases = value;
            }
        }

        public NsInfo(ModuleMember currentDocument)
        {
            ModuleMember = currentDocument;
        }
    }
}
