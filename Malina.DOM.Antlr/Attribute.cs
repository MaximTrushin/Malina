using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malina.DOM.Antlr
{
    public class Attribute : DOM.Attribute, IAntlrCharStreamConsumer
    {
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
                return _charStream.GetText(new Interval(_idInterval.a + 1, _idInterval.b));
            }

            set
            {
                base.Name = value;
            }
        }
    }
}
