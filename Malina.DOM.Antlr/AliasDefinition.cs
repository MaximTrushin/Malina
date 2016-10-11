using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Malina.DOM;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Malina.DOM.Antlr
{

    public interface IAntlrCharStreamConsumer
    {
        ICharStream CharStream{set;}
    }
    public class AliasDefinition : DOM.AliasDefinition, IAntlrCharStreamConsumer
    {
        private ICharStream _charStream;
        private Interval _aliasDefIDInterval;
        public ICharStream CharStream
        {
            set
            {
                _charStream = value;
            }
        }

        public Interval AliasDefIDInterval
        {
            set
            {
                _aliasDefIDInterval = value;
            }
        }

        public override string Name
        {
            get
            {
                if (base.Name != null) return base.Name;
                return _charStream.GetText(new Interval(_aliasDefIDInterval.a + 2, _aliasDefIDInterval.b));
            }

            set
            {
                base.Name = value;
            }
        }
    }
}
