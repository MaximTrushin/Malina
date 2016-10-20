using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malina.DOM.Antlr
{
    public class Element : DOM.Element, IAntlrCharStreamConsumer, IValueNode
    {
        private ICharStream _charStream;
        private Interval _idInterval;
        private List<Interval> _valueIntervals;

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
                return _charStream.GetText(new Interval(_idInterval.a, _idInterval.b));
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
                if (_valueIntervals == null) return base.Value;
                return GetValueFromIntervals();
            }
        }

        private string GetValueFromIntervals()
        {
            var _sb = new StringBuilder();
            foreach (var item in _valueIntervals)
            {
                if (item.a == -1) { _sb.AppendLine(); continue; };
                _sb.Append(_charStream.GetText(item));
            }

            return _sb.ToString();
        }
    }
}
