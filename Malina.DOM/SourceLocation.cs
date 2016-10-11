using System;

namespace Malina.DOM
{
    public class SourceLocation : IComparable<SourceLocation>, IEquatable<SourceLocation>
    {
        // Fields
        private readonly int _column;
        private readonly int _line;
        private readonly int _index;

        // Methods
        public SourceLocation(int line, int column, int index)
        {
            _line = line;
            _column = column;
            _index = index;
        }

        public int CompareTo(SourceLocation other)
        {
            int num = _line.CompareTo(other._line);
            if (num != 0)
            {
                return num;
            }
            return _column.CompareTo(other._column);
        }

        public bool Equals(SourceLocation other)
        {
            return (CompareTo(other) == 0);
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2})", _line, _column, _index);
        }

        // Properties
        public int Column
        {
            get
            {
                return _column;
            }
        }

        public virtual bool IsValid
        {
            get
            {
                return ((_line > 0) && (_column > 0));
            }
        }

        public int Line
        {
            get
            {
                return _line;
            }
        }

        public int Index
        {
            get
            {
                return _index;
            }
        }
    }


}