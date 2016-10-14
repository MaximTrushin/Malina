namespace Malina.DOM
{
    public class MalinaDepthFirstTransformer : IDomVisitor
    {
        private Node _resultingNode;

        public virtual void OnAlias(Alias node)
        {
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

        public virtual void OnDocument(Document node)
        {
            Visit(node.Namespaces);
            VisitNode(node.DocumentElement);
        }

        public virtual void OnElement(Element node)
        {
            Visit(node.Attributes);
            Visit(node.Entities);
        }

        public virtual void OnModule(Module node)
        {
            Visit(node.Members);
        }

        public virtual void OnNamespace(Namespace node)
        {
        }

        public virtual void OnNamespaceScope(NamespaceScope node)
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

        protected virtual void ReplaceCurrentNode(Node replacement)
        {
            _resultingNode = replacement;
        }

        protected virtual void ReplaceNode<T>(NodeCollection<T> items, Node current, Node replacement) where T : Node
        {
            if (replacement == null)
            {
                items.Remove((T)current);
            }
            else
            {
                items.Replace((T)current, (T)replacement);
            }
        }

        public Node Visit(Node node)
        {
            return VisitNode(node);
        }

        public void Visit<T>(NodeCollection<T> items) where T : Node
        {
            if (items == null) return;
            
            var nodes = items.ToArray();
           
            foreach (var currentNode in nodes)
            {
                var resultingNode = VisitNode(currentNode);
                if (currentNode != resultingNode)
                    ReplaceNode(items, currentNode, resultingNode);
            }
        }

        public virtual Node VisitNode(Node node)
        {
            if (node == null) return null;
            var saved = _resultingNode;
            _resultingNode = node;
            OnNode(node);
            var result = _resultingNode;
            _resultingNode = saved;
            return result;
        }
    }
}
