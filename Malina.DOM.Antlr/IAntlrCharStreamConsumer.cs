using Antlr4.Runtime;

namespace Malina.DOM.Antlr
{
    public interface IAntlrCharStreamConsumer
    {
        ICharStream CharStream { set; }
    }
}
