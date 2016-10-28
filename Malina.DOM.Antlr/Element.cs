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
                return _charStream.GetText(_idInterval);
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
                return GetValueFromIntervals(_charStream, _valueIntervals, _valueIndent, ValueType);
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

        public static string GetValueFromIntervals(ICharStream charStream, List<Interval> valueIntervals, int valueIndent, DOM.ValueType valueType)
        {
            var _sb = new StringBuilder();
            if (valueIndent > 0)
            {
                var value = charStream.GetText(valueIntervals[0]);
                var lines = value.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                var first = true;
                foreach (var item in lines)
                {
                    string s;
                    if (valueType == ValueType.OpenString)
                        s = item.TrimEnd(' ', '\t');
                    else
                        s = item;

                    if (first) {_sb.Append(s); first = false; continue; }

                    //Removing indents
                    if (s.Length <= valueIndent) { _sb.AppendLine();continue; }
                    _sb.AppendLine();
                    _sb.Append(s.Substring(valueIndent));                    
                }
            }
            else
                foreach (var item in valueIntervals)
                {
                    if (item.a == -1) continue;//skip interval if Empty String
                    string s = charStream.GetText(item);
                    if (valueType == ValueType.OpenString)
                        s = s.TrimEnd(' ', '\t');

                    _sb.Append(s);
                }

            return _sb.ToString();
        }
    }
}
