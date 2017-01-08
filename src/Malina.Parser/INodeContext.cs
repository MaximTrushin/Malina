using Antlr4.Runtime;
using Malina.DOM;
using Malina.DOM.Antlr;
using IValueNode = Malina.DOM.IValueNode;

namespace Malina.Parser
{
    public interface INodeContext<T> where T : Node
    {
        T Node { get; set; }
        void ApplyContext();
    }

    public static class NodeContextExtensions
    {
        public static T InitNode<T>(this INodeContext<T> ctx, Node parent) where T : Node, new()
        {
            if (ctx.Node == null)
            {
                var node = new T();
                parent?.AppendChild(node);
                ctx.Node = node;
                var csc = node as IAntlrCharStreamConsumer;
                if (csc != null)
                    csc.CharStream = ((ParserRuleContext) ctx).start.InputStream;
            }
            return ctx.Node;
        }

        /// <summary>
        /// This method is called by method EnterContext for Parameter or Alias object value 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctx"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static T InitValueNode<T>(this INodeContext<T> ctx, Node parent) where T : Node, new()
        {
            if (ctx.Node == null)
            {
                var node = new T();
                ((IValueNode) node).ValueType = DOM.ValueType.Empty;

                if (parent != null)
                {
                    ((DOM.Antlr.IValueNode) parent).ObjectValue = node;
                    ((IValueNode) parent).ValueType = DOM.ValueType.ObjectValue;
                }
                ctx.Node = node;
                var csc = node as IAntlrCharStreamConsumer;
                if (csc != null)
                    csc.CharStream = ((ParserRuleContext) ctx).start.InputStream;
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
            var isStartValid = start != null && new SourceLocation(start.Line, start.Column + 1, start.StartIndex).IsValid;
            if (stop != null)
            {
                node.end = new SourceLocation(stop.Line, stop.Column, stop.StartIndex);
                if (!isStartValid)
                    node.start = node.end;
            }
        }
    }
}
