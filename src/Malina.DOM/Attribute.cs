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
    public class Attribute : Entity, IValueNode, INsNode
    {
        // Fields
        public string Namespace;
        private object _objectValue;
        private ValueType _valueType;
        private string _nsPrefix;

        // Methods

        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnAttribute(this);
        }

        // Properties
        public virtual string Value
        {
            get
            {
                return ObjectValue != null ? (!(ObjectValue is Alias) ? ObjectValue.ToString() : "$" + ((Alias) ObjectValue).Name) : null;
            }
            set
            {
                _objectValue = value;
            }
        }

        public object ObjectValue
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

        public virtual string NsPrefix
        {
            get
            {
                return _nsPrefix;
            }

            set
            {
                _nsPrefix = value;
            }
        }

        public bool IsValueNode => true;
        public bool HasValue()
        {
            return ObjectValue != null || Value != null;
        }
    }


}