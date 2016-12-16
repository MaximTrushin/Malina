using System;

namespace Malina.DOM
{
    [Serializable]
    public class Scope : Entity
    {
        // Fields
        private NodeCollection<Entity> _entities;

        // Methods
        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnScope(this);
        }

        public override void AppendChild(Node child)
        {
            var item = child as Entity;
            if (item != null)
            {
                Entities.Add(item);
            }
            else
            {
                base.AppendChild(child);
            }
        }

        // Properties
 
        public NodeCollection<Entity> Entities
        {
            get { return _entities ?? (_entities = new NodeCollection<Entity>(this)); }
            set
            {
                if (value != _entities)
                {
                    value?.InitializeParent(this);
                    _entities = value;
                }
            }
        }
    }


}