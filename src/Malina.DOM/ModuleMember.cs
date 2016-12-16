using System;

namespace Malina.DOM
{
    [Serializable]
    public abstract class ModuleMember : Node
    {
        private NodeCollection<Namespace> _namespaces;

        // Methods

        public virtual Module Module => (Parent as Module);

        public virtual NodeCollection<Namespace> Namespaces
        {
            get { return _namespaces ?? (_namespaces = new NodeCollection<Namespace>(this)); }
            set
            {
                if (value != _namespaces)
                {
                    value?.InitializeParent(this);
                    _namespaces = value;
                }
            }
        }
    }


}