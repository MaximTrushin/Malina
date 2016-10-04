using System;
using System.Text;

namespace Malina.DOM
{
    [Serializable]
    public class Argument : Element
    {
        // Fields
        public bool IsValueArgument;
        public Alias OfAlias;

        // Methods
        public Argument()
        {
        }

        public Argument(Element e) : base(e.Namespace, e.Name, e.Entities, e.Attributes, e.Value, e.ObjectValue)
        {
            start = e.start;
            end = e.end;
        }

        public Argument(string ns, string name, string value, Alias ofAlias)
        {
            Namespace = ns;
            Name = name;
            Value = value;
            OfAlias = ofAlias;
        }

        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnArgument(this);
        }

        public override void Assign(Node node, bool shallow)
        {
            base.Assign(node, shallow);
            Argument argument = node as Argument;
            OfAlias = argument.OfAlias;
        }

        public override Node Clone()
        {
            Argument argument = new Argument();
            argument.Assign(this, false);
            return argument;
        }

        public override string ToString()
        {
            return new StringBuilder().Append(OfAlias.Name).Append(":").Append(Name).ToString();
        }
    }



}