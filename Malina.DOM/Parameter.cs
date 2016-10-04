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

        public Parameter(Element e) : base(e.Namespace, e.Name, e.Entities, e.Attributes, e.Value, e.ObjectValue)
        {
            base.start = e.start;
            base.end = e.end;
        }


        public Parameter(string ns, string name, string value, AliasDefinition ofAliasDefinition)
        {
            base.Namespace = ns;
            base.Name = name;
            base.Value = value;
            this.OfAliasDefinition = ofAliasDefinition;
        }

        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnParameter(this);
        }

        public override void Assign(Node node, bool shallow)
        {
            base.Assign(node, shallow);
            Parameter parameter = node as Parameter;
            this.OfAliasDefinition = parameter.OfAliasDefinition;
            this.IsValueParameter = parameter.IsValueParameter;
            this.EmptyAsDefault = parameter.EmptyAsDefault;
        }

        public override Node Clone()
        {
            Parameter parameter = new Parameter();
            parameter.Assign(this, false);
            return parameter;
        }

        public override string ToString()
        {
            return new StringBuilder().Append(this.OfAliasDefinition.Name).Append(":").Append(base.Name).ToString();
        }
    }


}