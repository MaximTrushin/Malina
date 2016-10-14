using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Malina.DOM.Antlr;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime;

namespace Malina.Parser
{
    public partial class MalinaParser
    {
        #region ALIAS_DEF NodeContext
        public partial class Alias_def_stmtContext : INodeContext<AliasDefinition>
        {
            public AliasDefinition Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ALIAS_DEF_ID();
                Node.IDInterval = new Interval( id.Symbol.StartIndex, id.Symbol.StopIndex);
            }
        }
        #endregion

        #region ATTRIBUTE NodeContext
        public partial class Attr_stmtContext : INodeContext<DOM.Antlr.Attribute>
        {
            public DOM.Antlr.Attribute Node { get; set; }
            
            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ATTRIBUTE_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
                var openValue = open_value();
                if (openValue != null)
                {
                    Node.IntervalSet = new IntervalSet();
                    foreach (var item in openValue.children)
                    {                        
                        Node.IntervalSet.Add((item.Payload as CommonToken).StartIndex, (item.Payload as CommonToken).StopIndex);
                    } 
                }
                else 
                {
                    var value = DQS();
                    if (value != null)
                    {
                        Node.IntervalSet = new IntervalSet((value.Payload as CommonToken).StartIndex + 1, (value.Payload as CommonToken).StopIndex - 1);
                    }
                }
            }
        }


        public partial class Value_attr_inlineContext : INodeContext<DOM.Antlr.Attribute>
        {
            public DOM.Antlr.Attribute Node { get; set; }
            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ATTRIBUTE_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
                var openValue = open_value();
                if (openValue != null)
                {
                    Node.IntervalSet = new IntervalSet();
                    foreach (var item in openValue.children)
                    {
                        Node.IntervalSet.Add((item.Payload as CommonToken).StartIndex, (item.Payload as CommonToken).StopIndex);
                    }
                }
                else
                {
                    var value = DQS();
                    if (value != null)
                    {
                        Node.IntervalSet = new IntervalSet((value.Payload as CommonToken).StartIndex + 1, (value.Payload as CommonToken).StopIndex - 1);
                    }
                }
            }
        }

        public partial class Empty_attr_inlineContext : INodeContext<DOM.Antlr.Attribute>
        {
            public DOM.Antlr.Attribute Node { get; set; }
            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ATTRIBUTE_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);                
            }
        }
        #endregion

        #region ELEMENT NodeContext
        public partial class Value_element_stmtContext : INodeContext<Element>
        {
            public Element Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ELEMENT_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
                var openValue = open_value();
                if (openValue != null)
                {
                    Node.IntervalSet = new IntervalSet();
                    foreach (var item in openValue.children)
                    {
                        Node.IntervalSet.Add((item.Payload as CommonToken).StartIndex, (item.Payload as CommonToken).StopIndex);
                    }
                }
                else
                {
                    var value = DQS();
                    if (value != null)
                    {
                        Node.IntervalSet = new IntervalSet((value.Payload as CommonToken).StartIndex + 1, (value.Payload as CommonToken).StopIndex - 1);
                    }
                }
            }

        }

        public partial class Empty_element_stmtContext : INodeContext<Element>
        {
            public Element Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ELEMENT_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);                
            }

        }

        public partial class Block_element_stmtContext : INodeContext<Element>
        {
            public Element Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ELEMENT_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }

        }
        #endregion

        #region PARAMETER NodeContext
        public partial class Empty_parameter_stmtContext : INodeContext<Parameter>
        {
            public Parameter Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = PARAMETER_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }
        }

        public partial class Value_parameter_stmtContext : INodeContext<Parameter>
        {
            public Parameter Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = PARAMETER_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
                var openValue = open_value();
                if (openValue != null)
                {
                    Node.IntervalSet = new IntervalSet();
                    foreach (var item in openValue.children)
                    {
                        Node.IntervalSet.Add((item.Payload as CommonToken).StartIndex, (item.Payload as CommonToken).StopIndex);
                    }
                }
                else
                {
                    var value = DQS();
                    if (value != null)
                    {
                        Node.IntervalSet = new IntervalSet((value.Payload as CommonToken).StartIndex + 1, (value.Payload as CommonToken).StopIndex - 1);
                    }
                }
            }
        }

        public partial class Block_parameter_stmtContext : INodeContext<Parameter>
        {
            public Parameter Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = PARAMETER_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }
        }
        #endregion

        #region ALIAS NodeContext
        public partial class Empty_alias_stmtContext : INodeContext<Alias>
        {
            public Alias Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ALIAS_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }
        }

        public partial class Block_alias_stmtContext : INodeContext<Alias>
        {
            public Alias Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ALIAS_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }
        }

        public partial class Value_alias_stmtContext : INodeContext<Alias>
        {
            public Alias Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ALIAS_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
                var openValue = open_value();
                if (openValue != null)
                {
                    Node.IntervalSet = new IntervalSet();
                    foreach (var item in openValue.children)
                    {
                        Node.IntervalSet.Add((item.Payload as CommonToken).StartIndex, (item.Payload as CommonToken).StopIndex);
                    }
                }
                else
                {
                    var value = DQS();
                    if (value != null)
                    {
                        Node.IntervalSet = new IntervalSet((value.Payload as CommonToken).StartIndex + 1, (value.Payload as CommonToken).StopIndex - 1);
                    }
                }
            }
        }

        #endregion

    }
}
