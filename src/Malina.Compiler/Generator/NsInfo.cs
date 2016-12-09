using Malina.DOM;
using System.Collections.Generic;

namespace Malina.Compiler
{
    /// <summary>
    /// Collects a list of used namespaces and aliases in the ModuleMember (Document or AliasDef)
    /// </summary>
    public class NsInfo
    {
        public ModuleMember ModuleMember { get; private set; }
        public bool AliasesResolved { get; internal set; }
        private List<Namespace> _namespaces;
        private List<Alias> _aliases;

        public List<Namespace> Namespaces
        {
            get
            {
                return _namespaces ?? (_namespaces = new List<Namespace>());
            }

            set
            {
                _namespaces = value;
            }
        }

        public List<Alias> Aliases
        {
            get
            {

                return _aliases ?? (_aliases = new List<Alias>());
            }

            set
            {
                _aliases = value;
            }
        }

        public NsInfo(ModuleMember currentDocument)
        {
            ModuleMember = currentDocument;
        }
    }
}
