using System;

namespace Malina.DOM
{
    [Serializable]
    public class Attribute : Node
    {
        // Fields
        public string Namespace;
        private object _objectValue;
        public ValueType ValueType;

        // Methods
        public Attribute()
        {
        }

        public Attribute(string ns, string name, object value)
        {
            Namespace = ns;
            Name = name;
            ObjectValue = value;
        }

        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnAttribute(this);
        }

        public override void Assign(Node node)
        {
            base.Assign(node);
            Attribute attribute = node as Attribute;
            Namespace = attribute.Namespace;
            Name = attribute.Name;
            ObjectValue = attribute.ObjectValue;
            ValueType = attribute.ValueType;
        }

        public override Node Clone()
        {
            Attribute attribute = new Attribute();
            attribute.Assign(this);
            return attribute;
        }

        // Properties
        public virtual string Value
        {
            get
            {
                return ((ObjectValue != null) ? (!(ObjectValue is Alias) ? ObjectValue.ToString() : ("$" + (ObjectValue as Alias).Name)) : null);
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
    }


}