using System;
using System.Collections.Generic;
using System.Linq;
using Malina.DOM;
using Malina.DOM.Antlr;

namespace Malina.Compiler
{
    public class NamespaceResolver
    {
        /// <summary>
        /// Class is used to collect a list of used namespace in the ModuleMember (Document or AliasDef)
        /// </summary>
        public class NsInfo
        {
            public ModuleMember ModuleMember { get; private set; }
            public bool AliasesResolved { get; internal set; }
            private List<DOM.Namespace> _namespaces;
            private List<DOM.Alias> _aliases;

            public List<DOM.Namespace> Namespaces
            {
                get
                {                    
                    return _namespaces??(_namespaces = new List<DOM.Namespace>());
                }

                set
                {
                    _namespaces = value;
                }
            }

            public List<DOM.Alias> Aliases
            {
                get
                {

                    return _aliases??(_aliases = new List<DOM.Alias>());
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

        private List<NsInfo> _moduleMembersNsInfo;
        private ModuleMember _currentModuleMember;
        private Module _currentModule;
        private readonly CompilerContext _context;
        private Stack<NsInfo> _aliasStack;

        public NamespaceResolver(CompilerContext context)
        {
            _context = context;
            _aliasStack = new Stack<NsInfo>();
        }

        private List<NsInfo> ModuleMembersNsInfo
        {
            get
            {
                if (_moduleMembersNsInfo == null) _moduleMembersNsInfo = new List<NsInfo>();
                return _moduleMembersNsInfo;
            }
            set
            {
                _moduleMembersNsInfo = value;
            }
        }


        public void ResolveAliasesInDocuments()
        {
            foreach(var nsInfo in ModuleMembersNsInfo)
            {
                if (nsInfo.ModuleMember is DOM.Document) ResolveAliasesInDocument(nsInfo);
            }
        }

        private void ResolveAliasesInDocument(NsInfo documentNsInfo)
        {
            foreach (var alias in documentNsInfo.Aliases)
            {
                NsInfo aliasNsInfo = ResolveAliasInDocument(alias, documentNsInfo);
                MergeNsInfo(documentNsInfo, aliasNsInfo);
            }            
        }

        private void MergeNsInfo(NsInfo destNsInfo, NsInfo nsInfo)
        {
            throw new NotImplementedException();
        }

        private NsInfo ResolveAliasInDocument(DOM.Alias alias, NsInfo documentNsInfo)
        {
            //Finding AliasDef
            var aliasDef = _context.AliasDefinitions[alias.Name];
            if(aliasDef == null)
            {
                //Report Error
                _context.Errors.Add(CompilerErrorFactory.AliasIsNotDefined(alias, (documentNsInfo.ModuleMember as DOM.Document).Module.FileName));
                return null;
            }
            return ResolveAliasesInAliasDefinition(aliasDef);          
        }

        private NsInfo ResolveAliasesInAliasDefinition(DOM.AliasDefinition aliasDef)
        {
            _currentModuleMember = aliasDef;
            var aliasDefNsInfo = CurrentModuleMemberNsInfo;

            if (aliasDefNsInfo.AliasesResolved) return aliasDefNsInfo;

            return ResolveAliasesInAliasDefinition(aliasDefNsInfo);
        }

        private NsInfo ResolveAliasesInAliasDefinition(NsInfo aliasDefNsInfo)
        {
            //Check if Alias is already being resolved (circular reference)

            if (_aliasStack.Any(n => n == aliasDefNsInfo))
            {
                //Report Error
                _context.Errors.Add(CompilerErrorFactory.AliasDefHasCircularReference(aliasDefNsInfo));
                return aliasDefNsInfo;
            }

            _aliasStack.Push(aliasDefNsInfo);

            foreach (var alias in aliasDefNsInfo.Aliases)
            {
                NsInfo aliasNsInfo = ResolveAliasInAliasDefinition(alias, aliasDefNsInfo);
                MergeNsInfo(aliasDefNsInfo, aliasNsInfo);
            }

            _aliasStack.Pop();

            aliasDefNsInfo.AliasesResolved = true;

            return aliasDefNsInfo;
        }

        private NsInfo ResolveAliasInAliasDefinition(DOM.Alias alias, NsInfo aliasDefNsInfo)
        {
            //Finding AliasDef
            var aliasDef = _context.AliasDefinitions[alias.Name];
            if (aliasDef == null)
            {
                //Report Error
                _context.Errors.Add(CompilerErrorFactory.AliasIsNotDefined(alias, (aliasDefNsInfo.ModuleMember as DOM.Document).Module.FileName));
                return null;
            }
            return ResolveAliasesInAliasDefinition(aliasDef);

        }

        private NsInfo _currentModuleMemberNsInfo;
        private NsInfo CurrentModuleMemberNsInfo
        {
            get
            {
                if (_currentModuleMemberNsInfo != null && _currentModuleMemberNsInfo.ModuleMember == _currentModuleMember) return _currentModuleMemberNsInfo;

                var result = ModuleMembersNsInfo.FirstOrDefault(n => n.ModuleMember == _currentModuleMember);

                if (result == null)
                {
                    result = new NsInfo(_currentModuleMember);
                    _currentModuleMemberNsInfo = result;
                    _moduleMembersNsInfo.Add(result);
                }
                return result;
            }
        }

        public void ProcessAlias(DOM.Antlr.Alias node)
        {
            CurrentModuleMemberNsInfo.Aliases.Add(node);
        }


        public void EnterDocument(DOM.Antlr.Document node)
        {
            _currentModuleMember = node;
        }

        public void EnterAliasDef(DOM.Antlr.AliasDefinition node)
        {
            _currentModuleMember = node;
        }

        public void EnterModule(Module node)
        {
            _currentModule = node;
            _currentModuleMember = null;
        }

        /// <summary>
        /// For the Namespace Prefix of the Node:
        /// - reports error if it is not defined
        /// - finds Namespace DOM object in the Module or ModuleMember and adds it to NsINFo of the current ModuleMember
        /// </summary>
        /// <param name="node"></param>
        internal void ProcessNsPrefix(Node node)
        {
            var nsPrefix = (node as INsNode).NsPrefix;

            if (nsPrefix != null && !nsPrefix.StartsWith("xml", StringComparison.OrdinalIgnoreCase))
            {
                var ns = LookupNamespace(nsPrefix);
                if (ns == null)
                {
                    _context.Errors.Add(CompilerErrorFactory.NsPrefixNotDefined(node, _currentModule.FileName));
                    return;
                }

                CurrentModuleMemberNsInfo.Namespaces.Add(ns);

            }
        }

        /// <summary>
        /// Looks up Namespace with the specified Prefix in the current ModuleMember first and then in the Module
        /// </summary>
        /// <param name="nsPrefix"></param>
        /// <returns></returns>
        private DOM.Namespace LookupNamespace(string nsPrefix)
        {
            DOM.Namespace ns = null;
            if ((ns = _currentModuleMember.Namespaces.FirstOrDefault(n => n.Name == nsPrefix)) != null)
                return ns;

            if ((ns = _currentModule.Namespaces.FirstOrDefault(n => n.Name == nsPrefix)) != null)
                return ns;

            return null;
        }

    }
}
