using System;

namespace Malina.DOM
{
    [Serializable]
    public class Namespace : Node
    {
        // Fields
        public bool Derived;
        public string[] Elements;
        public string Name;
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

        public override void Assign(Node node, bool shallow)
        {
            base.Assign(node, shallow);
            Namespace namespace2 = node as Namespace;
            Name = namespace2.Name;
            Url = namespace2.Url;
            Elements = namespace2.Elements;
        }

        public override Node Clone()
        {
            Namespace namespace2 = new Namespace();
            namespace2.Assign(this, false);
            return namespace2;
        }
    }


}