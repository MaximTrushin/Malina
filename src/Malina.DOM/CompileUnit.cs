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
            get { return _modules ?? (_modules = new NodeCollection<Module>(this)); }
            set
            {
                if (value == _modules) return;
                value?.InitializeParent(this);
                _modules = value;
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
            var item = child as Module;
            if (item != null)
            {
                Modules.Add(item);
            }
            else
            {
                base.AppendChild(child);
            }
        }
    }
}
