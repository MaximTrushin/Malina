using Malina.DOM;
using Malina.DOM.Antlr;
using System.Text;

namespace Malina.Parser.Tests
{
    class DOMPrinterVisitor: DepthFirstTransformer
    {
        private int _indent = 0;
        private StringBuilder _sb = new StringBuilder();

        public string Text
        {
            get { return _sb.ToString(); }
        }

        public override void OnAliasDefinition(DOM.AliasDefinition node)
        {
            
            _sb.Append(node.start.Line);
            _sb.Append(":");
            _sb.Append(node.start.Column);
            _sb.Append('\t', _indent);            
            _sb.Append("    AliasDefinition '");
            _sb.Append(node.Name);
            _sb.Append("'   :");
            _sb.AppendLine();
            _indent++;
            base.OnAliasDefinition(node);
            _indent--;
            _sb.AppendLine();
            
        }
        public override void OnAttribute(DOM.Attribute node)
        {            
            _sb.Append(node.start.Line);
            _sb.Append(":");
            _sb.Append(node.start.Column);
            _sb.Append('\t', _indent);            
            _sb.Append("    Attribute '");
            _sb.Append(node.Name);
            _sb.Append("'   :");
            base.OnAttribute(node);
            _sb.AppendLine();
        }
        

    }

}

