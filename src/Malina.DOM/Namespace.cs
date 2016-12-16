using System;

namespace Malina.DOM
{
    [Serializable]
    public class Namespace : Node, IValueNode
    {
        // Fields
        private string _value;
        private ValueType _valueType;

        public bool IsValueNode => true;

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
    }


}