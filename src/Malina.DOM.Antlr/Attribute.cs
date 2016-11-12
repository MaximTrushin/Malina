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
        private Interval _idInterval;
        private Interval _valueInterval = Interval.Invalid;
        private int _valueIndent;
        private int _nsSeparator = -2;

        public ICharStream CharStream
        {
            set
            {
                _charStream = value;
            }
        }

        public Interval IDInterval
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
                if (NsSeparator > 0)
                {
                    return _charStream.GetText(new Interval(NsSeparator, _idInterval.b));
                }
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
                return Element.GetValueFromValueInterval(_charStream, _valueInterval, _valueIndent, ValueType);
            }
        }

        public override void AppendChild(Node child)
        {
            ObjectValue = child;
        }
        public int ValueIndent
        {
            get
            {
                return _valueIndent;
            }

            set
            {
                _valueIndent = value;
            }
        }

        public int NsSeparator
        {
            get
            {
                if (_nsSeparator == -2)
                {
                    //Calsulate NsSeparator
                    _nsSeparator = Element.CalcNsSeparator(_charStream, _idInterval);
                }
                return _nsSeparator;
            }

            set
            {
                _nsSeparator = value;
            }
        }

        public override string NsPrefix
        {
            get
            {
                if (base.NsPrefix != null) return base.NsPrefix;
                if (NsSeparator > 0)
                {
                    return _charStream.GetText(new Interval(_idInterval.a + 1, NsSeparator - 2));
                }
                return null;
            }
        }
    }
}
