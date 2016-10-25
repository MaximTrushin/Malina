using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System.Text;
using System;
using System.Collections.Generic;

namespace Malina.DOM.Antlr
{
    public class Namespace : DOM.Namespace, IAntlrCharStreamConsumer, IValueNode
    {
        private ICharStream _charStream;
        private Interval _idInterval;
        private List<Interval> _valueIntervals;
        private int _valueIndent;

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

        public List<Interval> ValueIntervals
        {
            get
            {
                return _valueIntervals;
            }

            set
            {
                _valueIntervals = value;
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

        public object ObjectValue
        {
            get
            {
                return Value;
            }

            set
            {
                return;
            }
        }

        public override string Value
        {
            get
            {
                if (_valueIntervals == null) return base.Value;
                return Element.GetValueFromIntervals(_charStream, _valueIntervals, _valueIndent);
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
    }
}
