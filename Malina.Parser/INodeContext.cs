using Antlr4.Runtime;
using Malina.DOM;
using Malina.DOM.Antlr;

namespace Malina.Parser
{
    public interface INodeContext<T> where T : Node
    {
        T Node { get; set; }
        void ApplyContext();
    }

    public static class NodeContextExtensions
    {
        public static T InitNode<T>(this INodeContext<T> ctx, Node parent) where T : Node, IAntlrCharStreamConsumer, new()
        {
            if (ctx.Node == null)
            {
                var node = new T();
                if (parent != null)
                    parent.AppendChild(node);
                ctx.Node = node;
                node.CharStream = (ctx as ParserRuleContext).start.InputStream;
            }
            return ctx.Node;
        }

        public static T InitValueNode<T>(this INodeContext<T> ctx, Node parent) where T : Node, IAntlrCharStreamConsumer, new()
        {
            if (ctx.Node == null)
            {
                var node = new T();
                if (parent != null)
                    (parent as IValueNode).ObjectValue = node;
                ctx.Node = node;
                node.CharStream = (ctx as ParserRuleContext).start.InputStream;
            }
            return ctx.Node;
        }

        public static void SetNodeLocation<T>(this INodeContext<T> ctx) where T : Node
        {
            var context = (ParserRuleContext)ctx;
            var node = ctx.Node;
            SetNodeLocation(node, context.start, context.stop);
        }

        public static void SetNodeLocation(Node node, IToken start, IToken stop)
        {

            if (start != null)
                node.start = new SourceLocation(start.Line, start.Column, start.StartIndex);
            var isStartValid = (new SourceLocation(start.Line, start.Column + 1, start.StartIndex)).IsValid;
            if (stop != null)
            {
                node.end = new SourceLocation(stop.Line, stop.Column, stop.StartIndex);
                if (!isStartValid)
                    node.start = node.end;
            }
        }
    }
}
