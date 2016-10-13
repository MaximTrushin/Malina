﻿using Malina.DOM;
using Malina.DOM.Antlr;
using System.Text;

namespace Malina.Parser.Tests
{
    class DOMPrinterVisitor: MalinaDepthFirstTransformer
    {
        private int _indent = 0;
        private StringBuilder _sb = new StringBuilder();

        public string Text
        {
            get { return _sb.ToString(); }
        }

        public override void OnAliasDefinition(DOM.AliasDefinition node)
        {
            PrintNodeStart(node);
            _sb.Append(":");
            _sb.AppendLine();
            _indent++;
            base.OnAliasDefinition(node);
            _indent--;
            _sb.AppendLine();
            
        }
        public override void OnAttribute(DOM.Attribute node)
        {
            PrintNodeStart(node);
            _sb.Append("= `");
            _sb.Append(node.Value);
            _sb.Append("`");
            _sb.AppendLine();
        }

        public override void OnElement(DOM.Element node)
        {
            PrintNodeStart(node);

            if (node.ObjectValue != null)
                _sb.Append("= `");
                _sb.Append(node.Value);
            if (node.Attributes.Count + node.Entities.Count > 0)
            {
                _sb.AppendLine(":");
                _indent++;
            }

            base.OnElement(node);

            if (node.ObjectValue != null)
                _sb.Append("`");
            if (node.Attributes.Count + node.Entities.Count > 0)
            {
                _indent++;
            }
            _sb.AppendLine();
        }

        private void PrintNodeStart(DOM.Node node)
        {
            _sb.Append(node.start.Line);
            _sb.Append(":");
            _sb.Append(node.start.Column);
            _sb.Append('\t', _indent);
            _sb.Append("\t");
            _sb.Append(node.GetType().Name);
            _sb.Append(" `");
            _sb.Append(node.Name);
            _sb.Append("`");
        }

    }

}

