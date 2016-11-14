using System;
using System.Collections.Generic;
using System.Linq;

namespace Malina.DOM
{
    [Serializable]
    public class Module : Node
    {
        // Fields
        private NodeCollection<ModuleMember> _member;
        private Dictionary<string, Namespace> _namespaces;
        public string FileName;

        // Methods


        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnModule(this);
        }

        public override void AppendChild(Node child)
        {
            if (child is ModuleMember)
            {
                Members.Add((ModuleMember)child);
            }
            else if (child is Namespace)
            {
                Namespaces.Add(((Namespace)(child)).Name, (Namespace)child);
            }
            else
            {
                base.AppendChild(child);
            }
        }

        public override void Assign(Node node)
        {
            base.Assign(node);
            Module module = node as Module;
            Name = module.Name;
            Members.AssignNodes(module.Members);
            Namespaces = module.Namespaces.ToDictionary(entry => entry.Key, entry => entry.Value.Clone() as Namespace);
        }

        public override Node Clone()
        {
            Module module = new Module();
            module.Assign(this);
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

        public Dictionary<string, Namespace> Namespaces
        {
            get
            {
                if (_namespaces == null)
                {
                    _namespaces = new Dictionary<string, Namespace>();
                }
                return _namespaces;
            }
            set
            {
                if (value != _namespaces)
                {
                    _namespaces = value;
                }
            }
        }
    }


}