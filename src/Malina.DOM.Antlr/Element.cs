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
        private Interval _valueInterval = Interval.Invalid;
        private int _valueIndent;
        private int _nsSeparator;        

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

        public override string Value
        {
            get
            {
                if (base.Value != null) return base.Value;
                return GetValueFromValueInterval(_charStream, _valueInterval, _valueIndent, ValueType);
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
                return _nsSeparator;
            }

            set
            {
                _nsSeparator = value;
            }
        }

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
                if (valueType == ValueType.OpenString)
                    s = item.TrimEnd(' ', '\t'); //ignoring trailing whitespace for open strings
                else
                    s = item;

                if (first) {_sb.Append(s); first = false; continue; }

                //Removing indents
                if (s.Length <= valueIndent) { _sb.AppendLine();continue; }
                _sb.AppendLine();
                _sb.Append(s.Substring(valueIndent));                    
            }

            return _sb.ToString();
        }
    }
}
