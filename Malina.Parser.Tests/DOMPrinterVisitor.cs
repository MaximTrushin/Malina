using Malina.DOM;
using Malina.DOM.Antlr;
using System.Collections.Generic;
using System.Text;

namespace Malina.Parser.Tests
{
    class DOMPrinterVisitor: MalinaDepthFirstTransformer
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
            else
            {
                _sb.AppendLine();
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
                _sb.Append(node.Value);
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

