using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Malina.DOM.Antlr
{
    public class AliasDefinition : DOM.AliasDefinition, IAntlrCharStreamConsumer
    {
        private NodeCollection<Parameter> _parameters;
        private ICharStream _charStream;
        private Interval _idInterval;
        public ICharStream CharStream
        {
            set
            {
                _charStream = value;
            }
        }

        public Interval IDInterval
        {
            set
            {
                _idInterval = value;
            }
        }

        public override string Name
        {
            get
            {
                if (base.Name != null) return base.Name;
                return _charStream.GetText(new Interval(_idInterval.a + 2, _idInterval.b));
            }

            set
            {
                base.Name = value;
            }
        }

        public NodeCollection<Parameter> Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    _parameters = new NodeCollection<Parameter>(this);
                }
                return _parameters;
            }
            set
            {
                if (value != _parameters)
                {
                    _parameters = value;
                }
            }
        }
    }
}
