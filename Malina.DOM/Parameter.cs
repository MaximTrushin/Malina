using System;
using System.Text;

namespace Malina.DOM
{
    [Serializable]
    public class Parameter : Element
    {
        // Fields
        public bool EmptyAsDefault;
        public bool IsValueParameter;
        public AliasDefinition OfAliasDefinition;

        // Methods
        public Parameter()
        {
        }

        public Parameter(Element e) : base(e.Namespace, e.Name, e.Entities, e.Attributes, e.ObjectValue)
        {
            base.start = e.start;
            base.end = e.end;
        }


        public Parameter(string ns, string name, AliasDefinition ofAliasDefinition)
        {
            Namespace = ns;
            Name = name;
            OfAliasDefinition = ofAliasDefinition;
        }

        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnParameter(this);
        }

        public override void Assign(Node node)
        {
            base.Assign(node);
            Parameter parameter = node as Parameter;
            OfAliasDefinition = parameter.OfAliasDefinition;
            IsValueParameter = parameter.IsValueParameter;
            EmptyAsDefault = parameter.EmptyAsDefault;
        }

        public override Node Clone()
        {
            Parameter parameter = new Parameter();
            parameter.Assign(this);
            return parameter;
        }

        public override string ToString()
        {
            return new StringBuilder().Append(OfAliasDefinition.Name).Append(":").Append(Name).ToString();
        }
    }


}