using System;

namespace Malina.DOM
{
    [Serializable]
    public class CompileUnit: Node
    {
        // Fields
        private NodeCollection<Module> _modules;


        // Properties
        public NodeCollection<Module> Modules
        {
            get
            {
                if (_modules == null)
                {
                    _modules = new NodeCollection<Module>(this);
                }
                return _modules;
            }
            set
            {
                if (value != _modules)
                {
                    if (value != null)
                    {
                        value.InitializeParent(this);
                    }
                    _modules = value;
                }
            }
        }

        public override void Accept(IDomVisitor visitor)
        {
            visitor.OnCompileUnit(this);
        }

        public override Node Clone()
        {
            throw new NotImplementedException();
        }

        public override void AppendChild(Node child)
        {
            if (child is Module)
            {
                Modules.Add((Module)child);
            }
            else
            {
                base.AppendChild(child);
            }
        }
    }
}
