using System;

namespace Malina.DOM
{
    [Serializable]
    public enum ValueType
    {
        None = 0, //Node is not Value Node
        Empty, //Node is Value Node but Value is not defined (value parameter for ex.)
        DoubleQuotedString,
        SingleQuotedString,
        OpenString,
        ObjectValue, //Parameter or Alias
        Null, //Json null
        Number, // Json number literal
        Boolean, // Json boolean literal
        EmptyObject //Json empty object {}
    }


}
