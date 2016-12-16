using System;

namespace Malina.DOM
{
    [Serializable]
    public class Argument : Element
    {

        // Methods
        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnArgument(this);
        }

    }



}