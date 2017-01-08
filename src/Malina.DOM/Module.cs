using System;

namespace Malina.DOM
{
    [Serializable]
    public class Module : Node
    {
        // Fields
        private NodeCollection<ModuleMember> _member;
        private NodeCollection<Namespace> _namespaces;
        public string FileName;

        // Methods


        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnModule(this);
        }

        public override void AppendChild(Node child)
        {
            var item = child as ModuleMember;
            if (item != null)
            {
                Members.Add(item);
                return;
            }

            var ns = child as Namespace;
            if (ns != null)
            {
                Namespaces.Add(ns);
            }
            else
            {
                base.AppendChild(child);
            }
        }

        // Properties
        public NodeCollection<ModuleMember> Members
        {
            get { return _member ?? (_member = new NodeCollection<ModuleMember>(this)); }
            set
            {
                if (value != _member)
                {
                    value?.InitializeParent(this);
                    _member = value;
                }
            }
        }

        public NodeCollection<Namespace> Namespaces
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