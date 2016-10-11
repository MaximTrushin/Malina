using System;

namespace Malina.DOM
{
    [Serializable]
    public abstract class ModuleMember : Node
    {
        // Fields
        private string _name;
        private InArrayStoredStringProperty _nameProperty;

        // Methods
        protected ModuleMember()
        {
        }

        public string Name
        {
            get
            {
                if (_name != null) return _name;

                if (NameProperty != null) return NameProperty.Value;

                return null;
           }

            set
            {
                _name = value;
            }
        }

        public InArrayStoredStringProperty NameProperty
        {
            get
            {
                return _nameProperty;
            }

            set
            {
                _nameProperty = value;
            }
        }

        public override void Assign(Node node)
        {
            base.Assign(node);
            ModuleMember member = node as ModuleMember;
            _name = member._name;
            NameProperty = member.NameProperty;
        }
    }


}