using System;

namespace Malina.DOM
{
    [Serializable]
    public class Namespace : Node
    {
        // Fields
        public bool Derived;
        public string[] Elements;
        public string Url;

        // Methods
        public Namespace()
        {
        }

        public Namespace(string name, string url, string[] elements)
        {
            Name = name;
            Url = url;
            Elements = elements;
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
            Url = namespace2.Url;
            Elements = namespace2.Elements;
        }

        public override Node Clone()
        {
            Namespace namespace2 = new Namespace();
            namespace2.Assign(this);
            return namespace2;
        }
    }


}