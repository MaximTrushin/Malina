using System;
using System.Collections.Generic;

namespace Malina.DOM
{
    [Serializable]
    public class Alias : Entity, IValueNode, INsNode
    {
        // Fields
        private NodeCollection<Argument> _arguments;
        
        private NodeCollection<Entity> _entities;
        private object _objectValue;
        private ValueType _valueType;
        private string _nsPrefix;


        //Properties
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
                return ObjectValue != null ? (!(ObjectValue is Alias) ? ObjectValue.ToString() : "$" + ((Alias) ObjectValue).Name) : null;
            }
            set
            {
                _objectValue = value;
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

        // Methods


        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnAlias(this);
        }

        public override void AppendChild(Node child)
        {
            var item = child as Argument;
            if (item != null)
            {
                Arguments.Add(item);
            }
            else
            {
                Entities.Add((Entity) child);
            }
        }

        // Properties
        public NodeCollection<Argument> Arguments
        {
            get
            {
                return _arguments??(_arguments = new NodeCollection<Argument>(this));
            }
            set
            {
                if (value != _arguments)
                {
                    value?.InitializeParent(this);
                    _arguments = value;
                }
            }
        }

        public bool IsValueNode => _valueType != ValueType.None;
    }


}