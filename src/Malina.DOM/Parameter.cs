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

        public override void Assign(Node node)
        {
            base.Assign(node);
            Parameter parameter = node as Parameter;
        }

        public override Node Clone()
        {
            Parameter parameter = new Parameter();
            parameter.Assign(this);
            return parameter;
        }

        public override string ToString()
        {
            return new StringBuilder().Append("%").Append(Name).ToString();
        }
    }


}