namespace Malina.DOM
{
    public interface IValueNode
    {
        string Value { get; set; }
        object ObjectValue { get; set; }
        ValueType ValueType { get; set; }
        bool IsValueNode { get; }

    }
}
