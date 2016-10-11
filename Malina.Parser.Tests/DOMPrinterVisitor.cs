using Malina.DOM;
using Malina.DOM.Antlr;
using System.Text;

namespace Malina.Parser.Tests
{
    class DOMPrinterVisitor: DepthFirstTransformer
    {
        private StringBuilder _sb = new StringBuilder();

        public string Text
        {
            get { return _sb.ToString(); }
        }

        public override void OnAliasDefinition(DOM.AliasDefinition node)
        {
            _sb.Append(node.Name);
            base.OnAliasDefinition(node);
        }
    }
}
