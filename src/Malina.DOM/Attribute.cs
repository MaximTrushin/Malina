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