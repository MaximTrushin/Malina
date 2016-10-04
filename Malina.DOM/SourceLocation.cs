using System;

namespace Malina.DOM
{
    public class SourceLocation : IComparable<SourceLocation>, IEquatable<SourceLocation>
    {
        // Fields
        private readonly int _column;
        private readonly int _line;

        // Methods
        public SourceLocation(int line, int column)
        {
            this._line = line;
            this._column = column;
        }

        public int CompareTo(SourceLocation other)
        {
            int num = this._line.CompareTo(other._line);
            if (num != 0)
            {
                return num;
            }
            return this._column.CompareTo(other._column);
        }

        public bool Equals(SourceLocation other)
        {
            return (this.CompareTo(other) == 0);
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", this._line, this._column);
        }

        // Properties
        public int Column
        {
            get
            {
                return this._column;
            }
        }

        public virtual bool IsValid
        {
            get
            {
                return ((this._line > 0) && (this._column > 0));
            }
        }

        public int Line
        {
            get
            {
                return this._line;
            }
        }
    }


}