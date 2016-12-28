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
                return base.Name ??
                       _charStream.GetText(NsSeparator > 0 ? new Interval(NsSeparator, _idInterval.b) : _idInterval).Replace("..", ".");
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

                return NsSeparator > 0 ? _charStream.GetText(new Interval(_idInterval.a, NsSeparator - 2)) : null;
            }
        }

        public static int CalcNsSeparator(ICharStream charStream, Interval idInterval)
        {
            var s = charStream.GetText(idInterval);
            for (int i = 0; i < s.Length - 1; i++)
            {
                if (s[i] != '.') continue;
                if (s[i + 1] != '.') return idInterval.a + i + 1;

                i++;
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

        public static string GetValueFromValueInterval(ICharStream charStream, Interval valueInterval, int valueIndent, ValueType valueType)
        {
            if (valueInterval.Length == 0) return null; //Node has no value
            if (valueInterval.a == -1) return ""; //Node has empty value

            var _sb = new StringBuilder();

            //Splitting text. Getting array of text lines
            var lines = charStream.GetText(valueInterval).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            var first = true;
            var firstEmptyLine = true; //If true then previous line was not empty therefor newline shouldn't be added

            foreach (var item in lines)
            {
                string line = TrimEndOfOpenStringLine(item, valueType);

                if (first) {_sb.Append(line); first = false; continue; } //adding first line without appending new line symbol

                //Counting indent of line
                var lineIndent = line.TakeWhile(c => c == ' ' || c == '\t').Count();

                //Ignore dedented comments inside open string
                if (lineIndent < valueIndent && line.Substring(lineIndent).StartsWith("//")) continue;

                if (line.Length <= valueIndent) //this is just empty line
                {
                    if (valueType == ValueType.FreeOpenString)//Folded string
                    {
                        if (firstEmptyLine)
                        {
                            firstEmptyLine = false;
                            continue; //Ignore first empty line for folded string
                        }
                    }
                    _sb.AppendLine(); continue;
                }

                line = line.Substring(valueIndent);

                if (valueType == ValueType.FreeOpenString && firstEmptyLine) _sb.Append(" ");
                if (valueType != ValueType.FreeOpenString || !firstEmptyLine) _sb.AppendLine();
                firstEmptyLine = true; //reseting the flag for folded string logic
                _sb.Append(line);//Removing indents                    
            }

            return _sb.ToString();
        }

        private static string TrimEndOfOpenStringLine(string line, ValueType valueType)
        {
            if (valueType == ValueType.OpenString || valueType == ValueType.FreeOpenString || valueType == ValueType.Boolean || valueType == ValueType.Null || valueType == ValueType.Number)
                return line.TrimEnd(' ', '\t'); //ignoring trailing whitespace for open strings

            return line;
        }
    }
}
