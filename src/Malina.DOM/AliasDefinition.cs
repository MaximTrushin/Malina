using System;
using System.Collections.Generic;
using System.Linq;

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
            if (child is Parameter)
            {
                Entities.Add((Entity)child);
            }
            else if (child is Namespace)
            {
                Namespaces.Add((Namespace)child);
            }
            else if (child is Entity)
            {
                Entities.Add((Entity)child);
            }
            else
            {
                base.AppendChild(child);
            }
        }

        // Properties
        public NodeCollection<Entity> Entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = new NodeCollection<Entity>(this);
                }
                return _entities;
            }
            set
            {
                if (value != _entities)
                {
                    if (value != null)
                    {
                        value.InitializeParent(this);
                    }
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

        public bool IsValueNode
        {
            get
            {
                return _valueType != ValueType.None;
            }
        }
    }


}