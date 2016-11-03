using System;

namespace Malina.DOM
{
    [Serializable]
    public class SourceLocation : IComparable<SourceLocation>, IEquatable<SourceLocation>
    {
        // Fields
        public int Column;
        public int Line;
        public int Index;

        // Methods
        public SourceLocation(int line, int column, int index)
        {
            Line = line;
            Column = column;
            Index = index;
        }

        public SourceLocation() { }

        public int CompareTo(SourceLocation other)
        {
            int num = Line.CompareTo(other.Line);
            if (num != 0)
            {
                return num;
            }
            return Column.CompareTo(other.Column);
        }

        public bool Equals(SourceLocation other)
        {
            return (CompareTo(other) == 0);
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2})", Line, Column, Index);
        }

        // Properties

        public virtual bool IsValid
        {
            get
            {
                return ((Line > 0) && (Column > 0));
            }
        }


    }


}