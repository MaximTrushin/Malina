using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Malina.DOM.Antlr
{
    public class AliasDefinition : DOM.AliasDefinition, IAntlrCharStreamConsumer, IValueNode
    {
        private NodeCollection<Parameter> _parameters;

        public ICharStream CharStream { get; set; }

        public Interval IdInterval { get; set; }

        public override string Name
        {
            get
            {
                if (base.Name != null) return base.Name;
                return CharStream.GetText(new Interval(IdInterval.a + 2, IdInterval.b));
            }

            set
            {
                base.Name = value;
            }
        }

        public NodeCollection<Parameter> Parameters
        {
            get { return _parameters ?? (_parameters = new NodeCollection<Parameter>(this)); }
            set
            {
                if (_parameters != null && value != _parameters)
                {
                    _parameters = value;
                }
            }
        }

        public Interval ValueInterval { get; set; }


        public int ValueIndent { get; set; }

        public override string Value
        {
            get
            {
                if (base.Value != null) return base.Value;
                if (ValueType == ValueType.None) return null;
                return Element.GetValueFromValueInterval(CharStream, ValueInterval, ValueIndent, ValueType);
            }
        }

        private List<object> _interpolationItems;
        public List<object> InterpolationItems => _interpolationItems ?? (_interpolationItems = new List<object>());
    }
}
