using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malina.DOM.Antlr
{
    public class Element : DOM.Element, IAntlrCharStreamConsumer, IValueNode
    {
        private ICharStream _charStream;
        private Interval _idInterval;
        private int _valueIndent;
        private int _nsSeparator = -2;        

        public ICharStream CharStream
        {
            set
            {
                _charStream = value;
            }
        }

        public Interval IdInterval
        {
            set
            {
                _idInterval = value;
            }
        }

        public Interval ValueInterval { get; set; } = Interval.Invalid;

        public override string Name
        {
            get
            {
                if (base.Name != null) return base.Name;
                if (NsSeparator > 0)
                {
                    return _charStream.GetText(new Interval(NsSeparator, _idInterval.b));
                }
                return _charStream.GetText(_idInterval);
            }

            set
            {
                base.Name = value;
            }
        }

        public override string NsPrefix
        {
            get
            {
                if (base.NsPrefix != null) return base.NsPrefix;

                if (NsSeparator > 0)
                {
                    return _charStream.GetText(new Interval(_idInterval.a, NsSeparator - 2));
                }

                return null;
            }
        }

        public static int CalcNsSeparator(ICharStream _charStream, Interval _idInterval)
        {
            var s = _charStream.GetText(_idInterval);
            for (int i = 0; i < s.Length - 1; i++)
            {
                if(s[i] == '.')
                {
                    if (s[i + 1] != '.') return _idInterval.a + i + 1;

                    i++;
                }
            }
            return -1;
        }

        public override string Value
        {
            get
            {
                if (base.Value != null) return base.Value;
                return GetValueFromValueInterval(_charStream, ValueInterval, _valueIndent, ValueType);
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

        public int NsSeparator
        {
            get
            {
                if (_nsSeparator == -2)
                {
                    //Calculate NsSeparator
                    _nsSeparator = CalcNsSeparator(_charStream, _idInterval);
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

        public static string GetValueFromValueInterval(ICharStream charStream, Interval valueInterval, int valueIndent, DOM.ValueType valueType)
        {
            if (valueInterval.Length == 0) return null;
            if (valueInterval.a == -1) return "";
            var _sb = new StringBuilder();
            var value = charStream.GetText(valueInterval);
            var lines = value.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var first = true;
            foreach (var item in lines)
            {
                string s;
                if (valueType == ValueType.OpenString || valueType == ValueType.Boolean || valueType == ValueType.Null || valueType == ValueType.Number)
                    s = item.TrimEnd(' ', '\t'); //ignoring trailing whitespace for open strings
                else
                    s = item;

                if (first) {_sb.Append(s); first = false; continue; }

                //Counting indent of line
                var itemsIndent = item.TakeWhile(c => c == ' ' || c == '\t').Count();
                if (itemsIndent < valueIndent)
                {
                    //Ignore dedented comments inside open string
                    if (s.Substring(itemsIndent).StartsWith("//")) continue;
                }
                if (s.Length <= valueIndent) { _sb.AppendLine();continue; } //this is just empty line


                _sb.AppendLine();
                _sb.Append(s.Substring(valueIndent));//Removing indents                    
            }

            return _sb.ToString();
        }
    }
}
