using Malina.DOM.Antlr;
using Antlr4.Runtime.Misc;
using System.Collections.Generic;

namespace Malina.Parser
{
    public partial class MalinaParser
    {

        #region DOCUMENT NodeContext
        public partial class Document_stmtContext : INodeContext<Document>
        {
            public Document Node { get; set; }
            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = DOCUMENT_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }
        }

        public partial class Scope_stmtContext : INodeContext<Scope>
        {
            public Stack<DOM.Node> NodeStack;
            public Scope Node { get; set; }
            public void ApplyContext()
            {
                this.SetNodeLocation();
                
                var id = SCOPE_ID();
                if (id != null)
                {
                    var el = NodeStack.Peek();

                    NodeContextExtensions.SetNodeLocation(el, this.start, this.stop);
                    var dot = MalinaParserListener.FindChar(id.Symbol.StartIndex, id.Symbol.StopIndex, id.Symbol.InputStream, '.');
                    if (dot > -1)
                    {
                        (el as Element).IDInterval = new Interval(dot + 1, id.Symbol.StopIndex);
                        Node.IDInterval = new Interval(id.Symbol.StartIndex, dot - 1);
                    }
                    else
                    {
                        Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
                    }                    
                }                
            }

        }
        public partial class Scope_inlineContext : INodeContext<Scope>
        {
            public Stack<DOM.Node> NodeStack;
            public Scope Node { get; set; }
            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = SCOPE_ID();
                if (id != null)
                {
                    var el = NodeStack.Peek();

                    NodeContextExtensions.SetNodeLocation(el, this.start, this.stop);
                    var dot = MalinaParserListener.FindChar(id.Symbol.StartIndex, id.Symbol.StopIndex, id.Symbol.InputStream, '.');
                    if(dot > -1)
                    {
                        (el as Element).IDInterval = new Interval(dot + 1, id.Symbol.StopIndex);
                        Node.IDInterval = new Interval(id.Symbol.StartIndex, dot - 1);
                    }
                    else
                    {
                        Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
                    }
                }
            }
        }
        #endregion

        public partial class Namespace_declaration_stmtContext : INodeContext<Namespace>
        {
            public Namespace Node { get; set; }
            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = NAMESPACE_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }
        }
        

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

        #region STATEMENT Context
        public partial class Value_element_stmtContext : INodeContext<Element>
        {
            public Element Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ELEMENT_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
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

        #region INLINE Context
        public partial class Value_element_inlineContext : INodeContext<Element>
        {
            public Element Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ELEMENT_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }

        }

        public partial class Empty_element_inlineContext : INodeContext<Element>
        {
            public Element Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ELEMENT_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }

        }

        public partial class Block_element_inlineContext : INodeContext<Element>
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

        #endregion

        #region PARAMETER NodeContext
        #region STATEMENT Context
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

        #region INLINE Context
        public partial class Empty_parameter_inlineContext : INodeContext<Parameter>
        {
            public Parameter Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = PARAMETER_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }
        }

        public partial class Value_parameter_inlineContext : INodeContext<Parameter>
        {
            public Parameter Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = PARAMETER_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);

            }
        }

        public partial class Block_parameter_inlineContext : INodeContext<Parameter>
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

        #endregion

        #region ALIAS NodeContext
        #region STATEMENT Context
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

            }
        }
        #endregion

        #region INLINE Context
        public partial class Empty_alias_inlineContext : INodeContext<Alias>
        {
            public Alias Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ALIAS_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }
        }

        public partial class Block_alias_inlineContext : INodeContext<Alias>
        {
            public Alias Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ALIAS_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }
        }

        public partial class Value_alias_inlineContext : INodeContext<Alias>
        {
            public Alias Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ALIAS_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }
        }
        #endregion
        #endregion


        #region ARGUMENT NodeContext
        #region STATEMENT Context
        public partial class Empty_argument_stmtContext : INodeContext<Argument>
        {
            public Argument Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ARGUMENT_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }
        }

        public partial class Block_argument_stmtContext : INodeContext<Argument>
        {
            public Argument Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ARGUMENT_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }
        }

        public partial class Value_argument_stmtContext : INodeContext<Argument>
        {
            public Argument Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ARGUMENT_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }
        }
        #endregion

        #region INLINE Context
        public partial class Empty_argument_inlineContext : INodeContext<Argument>
        {
            public Argument Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ARGUMENT_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }
        }

        public partial class Block_argument_inlineContext : INodeContext<Argument>
        {
            public Argument Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ARGUMENT_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }
        }

        public partial class Value_argument_inlineContext : INodeContext<Argument>
        {
            public Argument Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ARGUMENT_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }
        }
        #endregion
        #endregion

        #region VALUEs NodeContext
        public partial class Parameter_object_value_inlineContext: INodeContext<Parameter>
        {
            public Parameter Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = PARAMETER_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }
        }

        public partial class Alias_object_value_inlineContext: INodeContext<Alias>
        {
            public Alias Node { get; set; }

            public void ApplyContext()
            {
                this.SetNodeLocation();
                var id = ALIAS_ID();
                Node.IDInterval = new Interval(id.Symbol.StartIndex, id.Symbol.StopIndex);
            }
        }
        
        #endregion


    }
}
