using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System.Collections.Generic;
using System.Text;

namespace Malina.DOM.Antlr
{
    public class Argument : DOM.Argument, IAntlrCharStreamConsumer, IValueNode
    {
        private ICharStream _charStream;
        private Interval _idInterval;
        private List<Interval> _valueintervals;

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
                return _valueintervals;
            }

            set
            {
                _valueintervals = value;
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
                if (_valueintervals == null) return base.Value;
                return GetValueFromIntervals();
            }
        }

        private string GetValueFromIntervals()
        {
            var _sb = new StringBuilder();
            var first = true;
            foreach (var item in _valueintervals)
            {
                if (!first) _sb.AppendLine();
                first = false;
                _sb.Append(_charStream.GetText(item));
            }

            return _sb.ToString();
        }
    }
}
