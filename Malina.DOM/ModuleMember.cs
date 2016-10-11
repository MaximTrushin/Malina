using System;

namespace Malina.DOM
{
    [Serializable]
    public abstract class ModuleMember : Node
    {
        // Fields
        private string _name;

        // Methods
        protected ModuleMember()
        {
        }

        public virtual string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public override void Assign(Node node)
        {
            base.Assign(node);
            ModuleMember member = node as ModuleMember;
            Name = member.Name;
        }
    }


}