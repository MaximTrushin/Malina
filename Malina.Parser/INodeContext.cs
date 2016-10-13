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
        public static void SetNodeLocation<T>(this INodeContext<T> ctx) where T : Node
        {
            var context = (ParserRuleContext)ctx;
            var node = ctx.Node;
            if (context.start != null)
                node.start = new SourceLocation(context.start.Line, context.start.Column, context.start.StartIndex);
            var isStartValid = (new SourceLocation(context.start.Line, context.start.Column + 1, context.start.StartIndex)).IsValid;
            if (context.stop != null)
            {
                node.end = new SourceLocation(context.stop.Line, context.stop.Column, context.stop.StartIndex);
                if (!isStartValid)
                    node.start = node.end;
            }
        }
    }
}
