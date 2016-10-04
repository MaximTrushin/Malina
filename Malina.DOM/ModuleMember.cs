using System;

namespace Malina.DOM
{
    [Serializable]
    public abstract class ModuleMember : Node
    {
        // Fields
        public string Name;

        // Methods
        protected ModuleMember()
        {
        }

        public override void Assign(Node node, bool shallow)
        {
            base.Assign(node, shallow);
            ModuleMember member = node as ModuleMember;
            this.Name = member.Name;
        }
    }


}