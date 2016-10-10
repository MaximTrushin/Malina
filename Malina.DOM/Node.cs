using System;
using System.Text;

namespace Malina.DOM
{
    [Serializable]
    public abstract class Node
    {
        // Fields
        private Node _parent;
        public SourceLocation end;
        public Module OwnerModule;
        public SourceLocation start;

        // Methods
        protected Node()
        {
        }

        public abstract void Accept(IDomVisitor visitor);
        public virtual void AppendChild(Node child)
        {
            throw new NotSupportedException(new StringBuilder("Cannot add ").Append(child.GetType().Name).Append(" in ").Append(GetType().Name).ToString());
        }

        public virtual void Assign(Node node)
        {
            _parent = node._parent;
            OwnerModule = node.OwnerModule;
            start = node.start;
            end = node.end;
        }

        public abstract Node Clone();
        internal void InitializeParent(Node parent)
        {
            _parent = parent;
        }

        // Properties
        public Node Parent
        {
            get
            {
                return _parent;
            }
        }
    }



}
