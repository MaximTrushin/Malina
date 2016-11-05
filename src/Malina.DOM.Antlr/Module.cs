using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace Malina.DOM.Antlr
{
    public class Module : DOM.Module, IAntlrCharStreamConsumer
    {
        public ICharStream CharStream
        {
            set {}
        }
    }
}
