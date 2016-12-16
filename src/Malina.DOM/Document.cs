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
            var item = child as Attribute;
            if (item != null)
            {
                Attributes.Add(item);
                return;
            }

            var entity = child as Entity;
            if (entity != null)
            {
                DocumentElement = entity;
                entity.InitializeParent(this);
                Entities.Add(entity);
                return;
            }

            var ns = child as Namespace;
            if (ns != null)
            {
                Namespaces.Add(ns);
                return;
            }
            base.AppendChild(child);

        }

        // Properties
        public NodeCollection<Entity> Entities => _entities ?? (_entities = new NodeCollection<Entity>(this));
    }


}