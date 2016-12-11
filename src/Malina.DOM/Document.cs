using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Malina.DOM
{
    [Serializable]
    public class Document : ModuleMember
    {
        // Fields
        private NodeCollection<Attribute> _attributes;
        private NodeCollection<Entity> _entities;
        
        public Entity DocumentElement;

        // Properties
        public NodeCollection<Attribute> Attributes => _attributes ?? (_attributes = new NodeCollection<Attribute>(this));

        // Methods
        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnDocument(this);
        }

        public override void AppendChild(Node child)
        {
            if (child is Attribute)
            {
                Attributes.Add((Attribute)child);
            }
            else if (child is Entity)
            {
                DocumentElement = (Entity)child;
                child.InitializeParent(this);
                Entities.Add((Entity)child);
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

        // Properties
        public NodeCollection<Entity> Entities => _entities ?? (_entities = new NodeCollection<Entity>(this));
    }


}