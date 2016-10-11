using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Malina.DOM.Antlr;
using Antlr4.Runtime.Misc;

namespace Malina.Parser
{
    public partial class MalinaParser
    {
        public partial class Alias_def_stmtContext : INodeContext<AliasDefinition>
        {
            public AliasDefinition Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ALIAS_DEF_ID();
                var inputStream = id.Symbol.InputStream;
                Node.AliasDefIDInterval = new Interval( id.Symbol.StartIndex, id.Symbol.StopIndex);
                var s = Node.Name;
                var i = 1;
            }
        }
    }
}
