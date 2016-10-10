using System;

namespace Malina.DOM
{
    [Serializable]
    public class Element : Entity
    {
        // Fields
        private NodeCollection<Attribute> _attributes;
        private NodeCollection<Entity> _entities;
        public bool DefaultNamespace;
        public bool IsValueElement;
        public string Namespace;
        public object ObjectValue;
        public string Value;
        public ValueType ValueType;

        // Methods
        public Element()
        {
        }

        public Element(string ns, string name, string value)
        {
            Namespace = ns;
            Name = name;
            Value = value;
        }

        public Element(string ns, string name, NodeCollection<Entity> entities, NodeCollection<Attribute> attributes, string value, object objectValue)
        {
            Namespace = ns;
            Name = name;
            Entities = entities;
            Attributes = attributes;
            Value = value;
            ObjectValue = objectValue;
        }

        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnElement(this);
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
            Element element = node as Element;
            Namespace = element.Namespace;
            Value = element.Value;
            ObjectValue = element.ObjectValue;
            DefaultNamespace = element.DefaultNamespace;
            ValueType = element.ValueType;
            Entities.AssignNodes(element.Entities);
            Attributes.AssignNodes(element.Attributes);

        }

        public override Node Clone()
        {
            Element element = new Element();
            element.Assign(this);
            return element;
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