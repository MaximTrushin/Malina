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

namespace Malina.DOM
{
    [Serializable]
    public class CompileUnit: Node
    {
        // Fields
        private NodeCollection<Module> _modules;


        // Properties
        public NodeCollection<Module> Modules
        {
            get { return _modules ?? (_modules = new NodeCollection<Module>(this)); }
            set
            {
                if (value == _modules) return;
                value?.InitializeParent(this);
                _modules = value;
            }
        }

        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnCompileUnit(this);
        }

        public override void AppendChild(Node child)
        {
            var item = child as Module;
            if (item != null)
            {
                Modules.Add(item);
            }
            else
            {
                base.AppendChild(child);
            }
        }
    }
}
