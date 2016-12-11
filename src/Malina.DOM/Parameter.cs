using System;
using System.Text;

namespace Malina.DOM
{
    [Serializable]
    public class Parameter : Element
    {
        // Methods
        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnParameter(this);
        }
 
        public override string ToString()
        {
            return new StringBuilder().Append("%").Append(Name).ToString();
        }
    }


}