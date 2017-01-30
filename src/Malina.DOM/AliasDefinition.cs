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
    public class AliasDefinition : ModuleMember, IValueNode
    {
        // Fields
        private NodeCollection<Entity> _entities;
        private object _objectValue;
        private ValueType _valueType;
        private string _value;

        // Methods
        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnAliasDefinition(this);
        }

        public override void AppendChild(Node child)
        {
            var item = child as Parameter;
            if (item != null)
            {
                Entities.Add(item);
                return;
            }

            var ns = child as Namespace;
            if (ns != null)
            {
                Namespaces.Add(ns);
                return;
            }

            var entity = child as Entity;
            if (entity != null)
            {
                Entities.Add(entity);
                return;
            }
            base.AppendChild(child);
        }

        // Properties
        public NodeCollection<Entity> Entities
        {
            get { return _entities ?? (_entities = new NodeCollection<Entity>(this)); }
            set
            {
                if (value != _entities)
                {
                    value?.InitializeParent(this);
                    _entities = value;
                }
            }
        }

        public virtual string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public virtual object ObjectValue
        {
            get
            {
                return _objectValue;
            }

            set
            {
                _objectValue = value;
            }
        }

        public ValueType ValueType
        {
            get
            {
                return _valueType;
            }

            set
            {
                _valueType = value;
            }
        }

        public bool IsValueNode => _valueType != ValueType.None && _valueType != ValueType.EmptyObject;
        public bool HasValue()
        {
            return ObjectValue != null || Value != null;
        }
    }
}