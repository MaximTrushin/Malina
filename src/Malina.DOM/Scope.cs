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
            if (child is Entity)
            {
                Entities.Add((Entity)child);
            }
            else
            {
                base.AppendChild(child);
            }
        }

        // Properties
 
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