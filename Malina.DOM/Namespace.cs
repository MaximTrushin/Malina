using System;

namespace Malina.DOM
{
    [Serializable]
    public class Namespace : Node
    {
        // Fields
        public bool Derived;
        private string value;

        // Methods
       public virtual string Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
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