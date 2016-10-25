using Antlr4.Runtime.Misc;
using System.Collections.Generic;

namespace Malina.DOM.Antlr
{
    public interface IValueNode
    {
        List<Interval> ValueIntervals { get; set; }
        object ObjectValue { get; set; }
        int ValueIndent { get; set; }
    }
}
