using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malina.DOM
{
    public class InArrayStoredStringProperty
    {
        private char[] _array;
        private int _startIndex;
        private int _stopIndex;

        public InArrayStoredStringProperty(char[] array, int startIndex, int stopIndex)
        {
            _array = array;
            _startIndex = startIndex;
            _stopIndex = stopIndex;
        }

        public char[] Stream
        {
            get
            {
                return _array;
            }
        }

        public int StartIndex
        {
            get
            {
                return _startIndex;
            }
        }

        public int StopIndex
        {
            get
            {
                return _stopIndex;
            }
        }

        public string Value
        {
            get
            {
                var length = _stopIndex - _startIndex + 1;
                return new string(_array, _startIndex, length);
            }
        }
    }
}
