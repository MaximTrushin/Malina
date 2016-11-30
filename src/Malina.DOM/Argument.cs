using System;

namespace Malina.DOM
{
    [Serializable]
    public class Argument : Element
    {

        // Methods
        public Argument()
        {
        }


        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnArgument(this);
        }

        public override void Assign(Node node)
        {
            base.Assign(node);
            Argument argument = node as Argument;
        }

        public override Node Clone()
        {
            Argument argument = new Argument();
            argument.Assign(this);
            return argument;
        }

    }



}