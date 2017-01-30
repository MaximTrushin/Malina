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
    public class Document : ModuleMember
    {
        // Fields
        private NodeCollection<Attribute> _attributes;
        private NodeCollection<Entity> _entities;
        
        public Entity DocumentElement;

        // Properties
        public NodeCollection<Attribute> Attributes => _attributes ?? (_attributes = new NodeCollection<Attribute>(this));

        // Methods
        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnDocument(this);
        }

        public override void AppendChild(Node child)
        {
            var item = child as Attribute;
            if (item != null)
            {
                Attributes.Add(item);
                return;
            }

            var entity = child as Entity;
            if (entity != null)
            {
                DocumentElement = entity;
                entity.InitializeParent(this);
                Entities.Add(entity);
                return;
            }

            var ns = child as Namespace;
            if (ns != null)
            {
                Namespaces.Add(ns);
                return;
            }
            base.AppendChild(child);

        }

        // Properties
        public NodeCollection<Entity> Entities => _entities ?? (_entities = new NodeCollection<Entity>(this));
    }


}