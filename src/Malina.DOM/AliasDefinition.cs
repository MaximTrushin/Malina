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