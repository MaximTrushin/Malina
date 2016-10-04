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
        public string Name;

        // Methods
        public Module()
        {
        }

        public Module(string name)
        {
            Name = name;
        }

        public Module(string name, NodeCollection<ModuleMember> members, NodeCollection<Namespace> namespaces)
        {
            Name = name;
            Members = members;
            Namespaces = namespaces;
        }

        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnModule(this);
        }

        public override void AppendChild(Node child)
        {
            child.OwnerModule = this;
            if (child is ModuleMember)
            {
                Members.Add((ModuleMember)child);
            }
            else if (child is Namespace)
            {
                Namespaces.Add((Namespace)child);
            }
            else
            {
                base.AppendChild(child);
            }
        }

        public override void Assign(Node node, bool shallow)
        {
            base.Assign(node, shallow);
            Module module = node as Module;
            Name = module.Name;
            if (!shallow)
            {
                Members.AssignNodes(module.Members);
                Namespaces.AssignNodes(module.Namespaces);
            }
        }

        public override Node Clone()
        {
            Module module = new Module();
            module.Assign(this, false);
            return module;
        }

        // Properties
        public NodeCollection<ModuleMember> Members
        {
            get
            {
                if (_member == null)
                {
                    _member = new NodeCollection<ModuleMember>(this);
                }
                return _member;
            }
            set
            {
                if (value != _member)
                {
                    if (value != null)
                    {
                        value.InitializeParent(this);
                    }
                    _member = value;
                }
            }
        }

        public NodeCollection<Namespace> Namespaces
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