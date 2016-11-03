using System;

namespace Malina.DOM
{
    [Serializable]
    public enum ValueType
    {
        None,
        DoubleQuotedString,
        SingleQuotedString,
        OpenString,
        ObjectValue, //Parameter or Alias
        Null, //Json null
        Number, // Json number literal
        Boolean, // Json boolean literal
    }


}
