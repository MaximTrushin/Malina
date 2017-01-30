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
using System.Text;

namespace Malina.DOM
{
    [Serializable]
    public abstract class Node
    {
        // Fields
        private string _name;
        private Node _parent;
        public SourceLocation start;
        public SourceLocation end;

        public virtual string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }
        // Methods
        public abstract void Accept(IDomVisitor visitor);
        public virtual void AppendChild(Node child)
        {
            throw new NotSupportedException(new StringBuilder("Cannot add ").Append(child.GetType().Name).Append(" in ").Append(GetType().Name).ToString());
        }

        internal void InitializeParent(Node parent)
        {
            _parent = parent;
        }

        // Properties
        public Node Parent => _parent;
    }
}
