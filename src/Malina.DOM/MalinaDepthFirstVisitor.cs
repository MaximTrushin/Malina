using System;
using System.Collections.Generic;

namespace Malina.DOM
{
    public class MalinaDepthFirstVisitor : IDomVisitor
    {

        public virtual void OnAlias(Alias node)
        {
            Visit(node.Attributes);
            Visit(node.Entities);
            Visit(node.Arguments);
        }

        public virtual void OnAliasDefinition(AliasDefinition node)
        {
            Visit(node.Namespaces);
            Visit(node.Attributes);
            Visit(node.Entities);
        }

        public virtual void OnArgument(Argument node)
        {
            Visit(node.Entities);
        }

        public virtual void OnAttribute(Attribute node)
        {

        }

        public void OnCompileUnit(CompileUnit node)
        {
            Visit(node.Modules);
        }
        public virtual void OnDocument(Document node)
        {
            Visit(node.Namespaces);
            Visit(node.Entities);
        }

        public virtual void OnElement(Element node)
        {
            Visit(node.Attributes);
            Visit(node.Entities);
        }

        public virtual void OnModule(Module node)
        {
            Visit(node.Namespaces);
            Visit(node.Members);
        }

        public virtual void OnNamespace(Namespace node)
        {
        }

        public virtual void OnScope(Scope node)
        {
            Visit(node.Attributes);
            Visit(node.Entities);
        }

        protected virtual void OnNode(Node node)
        {
            node.Accept(this);
        }

        public virtual void OnParameter(Parameter node)
        {
            Visit(node.Attributes);
            Visit(node.Entities);
        }


        public void Visit(Node node)
        {
            OnNode(node);
        }

        public void Visit<T>(IEnumerable<T> items) where T : Node
        {
            if (items == null) return;
            
            foreach (var node in items)
                OnNode(node);

        }


    }
}
