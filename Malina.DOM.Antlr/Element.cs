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
                return GetValueFromIntervals(_charStream, _valueIntervals, _valueIndent);
            }
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

        public static string GetValueFromIntervals(ICharStream charStream, List<Interval> valueIntervals, int valueIndent)
        {
            var _sb = new StringBuilder();
            if (valueIndent > 0)
            {
                var value = charStream.GetText(valueIntervals[0]);
                var lines = value.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                var first = true;
                foreach (var item in lines)
                {
                    if (first) {_sb.Append(item); first = false; continue; }
                    if (item.Length <= valueIndent) { _sb.AppendLine();continue; }
                    _sb.AppendLine();
                    _sb.Append(item.Substring(valueIndent));                    
                }
            }
            else
                foreach (var item in valueIntervals)
                {
                    if (item.a == -1) { _sb.AppendLine(); continue; };
                    _sb.Append(charStream.GetText(item));
                }

            return _sb.ToString();
        }
    }
}
