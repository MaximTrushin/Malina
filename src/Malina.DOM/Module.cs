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
    public class Module : Node
    {
        // Fields
        private NodeCollection<ModuleMember> _member;
        private NodeCollection<Namespace> _namespaces;
        public string FileName;

        // Methods


        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnModule(this);
        }

        public override void AppendChild(Node child)
        {
            var item = child as ModuleMember;
            if (item != null)
            {
                Members.Add(item);
                return;
            }

            var ns = child as Namespace;
            if (ns != null)
            {
                Namespaces.Add(ns);
            }
            else
            {
                base.AppendChild(child);
            }
        }

        // Properties
        public NodeCollection<ModuleMember> Members
        {
            get { return _member ?? (_member = new NodeCollection<ModuleMember>(this)); }
            set
            {
                if (value != _member)
                {
                    value?.InitializeParent(this);
                    _member = value;
                }
            }
        }

        public NodeCollection<Namespace> Namespaces
        {
            get { return _namespaces ?? (_namespaces = new NodeCollection<Namespace>(this)); }
            set
            {
                if (value != _namespaces)
                {
                    value?.InitializeParent(this);
                    _namespaces = value;
                }
            }
        }
    }


}