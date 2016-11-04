using Malina.DOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            throw new NotImplementedException();
        }

        public override Node Clone()
        {
            throw new NotImplementedException();
        }
    }
}
