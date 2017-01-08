using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System.Collections.Generic;

namespace Malina.DOM.Antlr
{
    public class Alias: DOM.Alias, IAntlrCharStreamConsumer, IValueNode
    {
        private ICharStream _charStream;
        private Interval _idInterval;
        private Interval _valueInterval = Interval.Invalid;

        public ICharStream CharStream
        {
            set
            {
                _charStream = value;
            }
        }

        public AliasDefinition AliasDefinition { get; set; }

        public Interval IdInterval
        {
            set
            {
                _idInterval = value;
            }
        }

        public Interval ValueInterval
        {
            get
            {
                return _valueInterval;
            }

            set
            {
                _valueInterval = value;
            }
        }

        public override string Name
        {
            get
            {
                if (base.Name != null) return base.Name;
                return _charStream.GetText(new Interval(_idInterval.a + 1, _idInterval.b));
            }

            set
            {
                base.Name = value;
            }
        }

        public override string Value
        {
            get
            {
                if (base.Value != null) return base.Value;
                return Element.GetValueFromValueInterval(_charStream, _valueInterval, ValueIndent, ValueType);
            }
        }

        public int ValueIndent { get; set; }

        private List<object> _interpolationItems;
        public List<object> InterpolationItems => _interpolationItems ?? (_interpolationItems = new List<object>());

        
    }
}
