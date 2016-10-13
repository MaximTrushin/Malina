using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malina.DOM.Antlr
{
    public class Element : DOM.Element, IAntlrCharStreamConsumer
    {
        private ICharStream _charStream;
        private Interval _idInterval;
        private IntervalSet _intervalSet;

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

        public IntervalSet IntervalSet
        {
            get
            {
                return _intervalSet;
            }

            set
            {
                _intervalSet = value;
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
                if (_intervalSet == null) return base.Value;
                return GetValueFromIntervals();
            }
        }

        private string GetValueFromIntervals()
        {
            var _sb = new StringBuilder();
            var first = true;
            foreach (var item in _intervalSet.GetIntervals())
            {
                if (!first) _sb.AppendLine();
                first = false;
                _sb.Append(_charStream.GetText(item));
            }

            return _sb.ToString();
        }
    }
}
