using Antlr4.Runtime;
using Malina.DOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                //if (parent != null)
                //    parent.AppendChild(node);
                ctx.Node = node;
            }
            return ctx.Node;
        }
        public static void SetNodeLocation<T>(this INodeContext<T> ctx) where T : Node
        {
            var context = (ParserRuleContext)ctx;
            var node = ctx.Node;
            if (context.start != null)
                node.start = new SourceLocation(context.start.Line, context.start.Column);
            var isStartValid = (new SourceLocation(context.start.Line, context.start.Column + 1)).IsValid;
            if (context.stop != null)
            {
                node.end = new SourceLocation(context.stop.Line, context.stop.Column);
                if (!isStartValid)
                    node.start = node.end;
            }
        }
    }
}
