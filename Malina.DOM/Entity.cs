using System;

namespace Malina.DOM
{
    [Serializable]
    public abstract class Entity : Node
    {
        // Fields
        public string Name;

        // Methods
        protected Entity()
        {
        }

        public override void Assign(Node node, bool shallow)
        {
            base.Assign(node, shallow);
            Entity entity = node as Entity;
            this.Name = entity.Name;
        }
    }


}