using System;

namespace Malina.DOM
{
    [Serializable]
    public abstract class ModuleMember : Node
    {
        private NodeCollection<Namespace> _namespaces;

        // Methods
        protected ModuleMember()
        {
        }

        public virtual Module Module
        {
            get
            {
                return (Parent as Module);
            }
        }

        public override void Assign(Node node)
        {
            base.Assign(node);
        }

        public virtual NodeCollection<Namespace> Namespaces
        {
            get
            {
                if (_namespaces == null)
                {
                    _namespaces = new NodeCollection<Namespace>(this);
                }
                return _namespaces;
            }
            set
            {
                if (value != _namespaces)
                {
                    if (value != null)
                    {
                        value.InitializeParent(this);
                    }
                    _namespaces = value;
                }
            }
        }
    }


}