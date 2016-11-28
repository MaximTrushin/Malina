using Malina.DOM;
using System.Collections.Generic;
using System.Text;
using Malina.DOM.Antlr;

namespace Malina.Parser.Tests
{
    public class DOMPrinterVisitor: MalinaDepthFirstVisitor
    {
        private int _indent = 0;
        private StringBuilder _sb = new StringBuilder();
        private Stack<bool> _valueNodeExpected = new Stack<bool>();

        public DOMPrinterVisitor():base()
        {
            _valueNodeExpected.Push(false);
        }

        public string Text
        {
            get { return _sb.ToString(); }
        }

        public override void OnNamespace(DOM.Namespace node)
        {
            PrintNodeStart(node);
            _sb.Append("= `");
            _sb.Append(node.Value);
            _sb.Append("`");
            _sb.AppendLine();
        }

        public override void OnScope(DOM.Scope node)
        {
            PrintNodeStart(node);
            _sb.Append(":");
            _sb.AppendLine();
            _indent++;
            base.OnScope(node);
            _indent--;
        }

        public override void OnDocument(DOM.Document node)
        {
            PrintNodeStart(node);
            _sb.Append(":");
            _sb.AppendLine();
            _indent++;
            base.OnDocument(node);
            _indent--;
            _sb.AppendLine();

        }
        public override void OnAliasDefinition(DOM.AliasDefinition node)
        {
            PrintNodeStart(node);
            if (node.ObjectValue is Node)
            {
                _sb.Append("= ");
                _valueNodeExpected.Push(true);
                Visit(node.ObjectValue as Node);
                _valueNodeExpected.Pop();
            }
            else if (node.Value != null)
            {
                _sb.Append("= `");
                PrintValue(node);
                _sb.Append("`");
            }

            if (node.Attributes.Count + node.Entities.Count > 0)
            {
                _sb.AppendLine(":");
                _indent++;
            }
            base.OnAliasDefinition(node);
            if (node.Attributes.Count + node.Entities.Count > 0)
            {
                _indent--;
            }
            _sb.AppendLine();

        }
        public override void OnAttribute(DOM.Attribute node)
        {
            PrintNodeStart(node);
            if (node.ObjectValue is Node)
            {
                _sb.Append("= ");
                _valueNodeExpected.Push(true);
                Visit(node.ObjectValue as Node);
                _valueNodeExpected.Pop();
            }
            else if (node.Value != null)
            {
                _sb.Append("= `");
                _sb.Append(node.Value);
                _sb.Append("`");
            }
            _sb.AppendLine();
        }

        public override void OnArgument(DOM.Argument node)
        {
            PrintNodeStart(node);
            if (node.ObjectValue is Node)
            {
                _sb.Append("= ");
                _valueNodeExpected.Push(true);
                Visit(node.ObjectValue as Node);
                _valueNodeExpected.Pop();
                _sb.AppendLine();
            }
            else if (node.Value != null)
            {
                _sb.Append("= `");
                _sb.Append(node.Value);
                _sb.Append("`");
                _sb.AppendLine();
            }
            if (node.Attributes.Count + node.Entities.Count > 0)
            {
                _sb.AppendLine(":");
                _indent++;
            }
            base.OnArgument(node);

            if (node.Attributes.Count + node.Entities.Count > 0)
            {
                _indent--;
            }
        }
        public override void OnElement(DOM.Element node)
        {
            PrintNodeStart(node);
            if (node.ObjectValue is Node)
            {
                _sb.Append("= ");
                _valueNodeExpected.Push(true);
                Visit(node.ObjectValue as Node);
                _valueNodeExpected.Pop();
            }
            else if(node.Value != null)
            {
                _sb.Append("= `");
                PrintValue(node);
                _sb.Append("`");
            }
            if (node.Attributes.Count + node.Entities.Count > 0)
            {
                _sb.AppendLine(":");
                _indent++;
            }

            base.OnElement(node);

            if (node.Attributes.Count + node.Entities.Count > 0)
            {
                _indent--;
            }
            else
            {
                _sb.AppendLine();
            }
        }

        private void PrintValue(DOM.IValueNode node)
        {
            _sb.Append(node.Value.Replace("\r\n", "\n").Replace("\n", "\\n").Replace("\t", "\\t"));
        }

        public override void OnParameter(DOM.Parameter node)
        {
            if (_valueNodeExpected.Peek())
            {
                _sb.Append("\t");
                _sb.Append(node.GetType().Name);
                _sb.Append(" `");
                _sb.Append(node.Name);
                _sb.Append("`");

                if (node.Value != null)
                {
                    _sb.Append("= `");
                    _sb.Append(node.Value);
                    _sb.Append("`");
                }
            }
            else PrintNodeStart(node);

            if (node.Attributes.Count + node.Entities.Count > 0)
            {                
                _sb.AppendLine(":");
                _indent++;
            }

            base.OnParameter(node);

            if (node.Attributes.Count + node.Entities.Count > 0)
            {
                _indent--;
            }
            else if (!_valueNodeExpected.Peek())
            {
                _sb.AppendLine();
            }
        }

        public override void OnAlias(DOM.Alias node)
        {
            if (_valueNodeExpected.Peek())
            {
                _sb.Append("\t");
                _sb.Append(node.GetType().Name);
                _sb.Append(" `");
                _sb.Append(node.Name);
                _sb.Append("`");

                if (node.Value != null)
                {
                    _sb.Append("= `");
                    _sb.Append(node.Value);
                    _sb.Append("`");
                }
            }
            else PrintNodeStart(node);

            if (node.Attributes.Count + node.Entities.Count + node.Arguments.Count > 0)
            {
                _sb.AppendLine(":");
                _indent++;
            }

            base.OnAlias(node);

            if (node.Attributes.Count + node.Entities.Count + node.Arguments.Count > 0)
            {
                _indent--;
            }
            else if (!_valueNodeExpected.Peek())
            {
                _sb.AppendLine();
            }
        }

        private void PrintNodeStart(DOM.Node node)
        {
            _sb.Append(node.start.Line.ToString().PadLeft(2, '0'));
            _sb.Append(":");
            _sb.Append(node.start.Column.ToString().PadLeft(2, '0'));
            _sb.Append('\t', _indent);
            _sb.Append("\t");
            _sb.Append(node.GetType().Name);
            _sb.Append(" `");
            PrintNsPrefix(node);
            _sb.Append(node.Name);
            _sb.Append("`");
        }

        private void PrintNsPrefix(Node node)
        {
            if (node is DOM.Antlr.Element)
            {
                var el = node as DOM.Antlr.Element;
                if (el.NsPrefix != null)
                {
                    _sb.Append(el.NsPrefix);
                    _sb.Append(".");
                }
            }

            if (node is DOM.Antlr.Attribute)
            {
                var at = node as DOM.Antlr.Attribute;
                if (at.NsPrefix != null)
                {
                    _sb.Append(at.NsPrefix);
                    _sb.Append(".");
                }
            }

        }
    }

}

