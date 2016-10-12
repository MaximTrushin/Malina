using System;

namespace Malina.DOM
{
    [Serializable]
    public class Attribute : Node
    {
        // Fields
        public string Namespace;
        public object ObjectValue;
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
        public string Value
        {
            get
            {
                return ((ObjectValue != null) ? (!(ObjectValue is Alias) ? ObjectValue.ToString() : ("$" + (ObjectValue as Alias).Name)) : null);
            }
        }

    }


}