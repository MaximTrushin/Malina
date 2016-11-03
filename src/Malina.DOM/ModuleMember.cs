using System;

namespace Malina.DOM
{
    [Serializable]
    public abstract class ModuleMember : Node
    {
        // Methods
        protected ModuleMember()
        {
        }

        

        public override void Assign(Node node)
        {
            base.Assign(node);
            ModuleMember member = node as ModuleMember;
            Name = member.Name;
        }
    }


}