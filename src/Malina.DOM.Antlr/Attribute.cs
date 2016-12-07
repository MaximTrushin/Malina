using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malina.DOM.Antlr
{
    public class Attribute : DOM.Attribute, IAntlrCharStreamConsumer, IValueNode
    {
        private ICharStream _charStream;
        private int _nsSeparator = -2;

        public ICharStream CharStream
        {
            set
            {
                _charStream = value;
            }
        }

        public Interval IdInterval { get; set; }

        public Interval ValueInterval { get; set; } = Interval.Invalid;

        public override string Name
        {
            get
            {
                if (base.Name != null) return base.Name;
                if (NsSeparator > 0)
                {
                    return _charStream.GetText(new Interval(NsSeparator, IdInterval.b));
                }
                return _charStream.GetText(new Interval(IdInterval.a + 1, IdInterval.b));


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
                return Element.GetValueFromValueInterval(_charStream, ValueInterval, ValueIndent, ValueType);
            }
        }

        public override void AppendChild(Node child)
        {
            ObjectValue = child;
        }
        public int ValueIndent { get; set; }

        public int NsSeparator
        {
            get
            {
                if (_nsSeparator == -2)
                {
                    //Calculate NsSeparator
                    _nsSeparator = Element.CalcNsSeparator(_charStream, IdInterval);
                }
                return _nsSeparator;
            }

            set
            {
                _nsSeparator = value;
            }
        }

        private List<object> _interpolationItems;
        public List<object> InterpolationItems => _interpolationItems ?? (_interpolationItems = new List<object>());

        public override string NsPrefix
        {
            get
            {
                if (base.NsPrefix != null) return base.NsPrefix;
                return NsSeparator > 0 ? _charStream.GetText(new Interval(IdInterval.a + 1, NsSeparator - 2)) : null;
            }
        }
    }
}
