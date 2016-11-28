using System;

namespace Malina.DOM
{
    [Serializable]
    public class Namespace : Node, IValueNode
    {
        // Fields
        private string _value;
        private ValueType _valueType;

        public bool IsValueNode
        {
            get
            {
                return true;
            }
        }

        public object ObjectValue
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value as string;
            }
        }

        // Methods
        public virtual string Value
        {
            get
            {
                return _value;
            }

            set
            {
                this._value = value;
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

        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnNamespace(this);
        }

        public override void Assign(Node node)
        {
            base.Assign(node);
            Namespace namespace2 = node as Namespace;
            Name = namespace2.Name;
            Value = namespace2.Value;
        }

        public override Node Clone()
        {
            Namespace namespace2 = new Namespace();
            namespace2.Assign(this);
            return namespace2;
        }
    }


}