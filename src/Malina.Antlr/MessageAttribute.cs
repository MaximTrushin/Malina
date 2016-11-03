using System;

namespace Malina.Parser
{
    internal class MessageAttribute : Attribute
    {
        public string Text;

        public MessageAttribute(string v)
        {
            Text = v;
        }
    }
}