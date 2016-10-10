using System;

namespace Malina.DOM
{
    [Serializable]
    public class NamespaceScope : Entity
    {
        // Fields
        private NodeCollection<Attribute> _attributes;
        private NodeCollection<Entity> _entities;

        // Methods
        protected NamespaceScope()
        {
        }

        public NamespaceScope(string name)
        {
            Name = name;
        }

        public NamespaceScope(string name, NodeCollection<Entity> entities, NodeCollection<Attribute> attributes)
        {
            Name = name;
            Entities = entities;
            Attributes = attributes;
        }

        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnNamespaceScope(this);
        }

        public override void AppendChild(Node child)
        {
            child.OwnerModule = OwnerModule;
            if (child is Attribute)
            {
                Attributes.Add((Attribute)child);
            }
            else if (child is Entity)
            {
                Entities.Add((Entity)child);
            }
            else
            {
                base.AppendChild(child);
            }
        }

        public override void Assign(Node node)
        {
            base.Assign(node);
            NamespaceScope scope = node as NamespaceScope;
            Name = scope.Name;
            Entities.AssignNodes(scope.Entities);
            Attributes.AssignNodes(scope.Attributes);

        }

        public override Node Clone()
        {
            NamespaceScope scope = new NamespaceScope();
            scope.Assign(this);
            return scope;
        }

        // Properties
        public NodeCollection<Attribute> Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    _attributes = new NodeCollection<Attribute>(this);
                }
                return _attributes;
            }
            set
            {
                if (value != _attributes)
                {
                    if (value != null)
                    {
                        value.InitializeParent(this);
                    }
                    _attributes = value;
                }
            }
        }

        public NodeCollection<Entity> Entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = new NodeCollection<Entity>(this);
                }
                return _entities;
            }
            set
            {
                if (value != _entities)
                {
                    if (value != null)
                    {
                        value.InitializeParent(this);
                    }
                    _entities = value;
                }
            }
        }
    }


}