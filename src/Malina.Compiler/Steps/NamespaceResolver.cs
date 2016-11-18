using System;
using System.Collections.Generic;
using System.Linq;
using Malina.DOM;
using Malina.DOM.Antlr;
using System.IO;

namespace Malina.Compiler
{
    public class NamespaceResolver
    {
        /// <summary>
        /// Collects a list of used namespaces and aliases in the ModuleMember (Document or AliasDef)
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

        /// <summary>
        /// NsInfo for all Module Members (Documents and AliasDef)
        /// </summary>
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


        public void ResolveAliasesAndDoChecks()
        {
            foreach(var nsInfo in ModuleMembersNsInfo)
            {
                CheckDocument(nsInfo);

                ResolveAliases(nsInfo);
            }
        }

        private void ResolveAliases(NsInfo nsInfo)
        {
            foreach (var alias in nsInfo.Aliases)
            {
                NsInfo aliasNsInfo = ResolveAliasInDocument(alias, nsInfo);
                if (aliasNsInfo == null) continue;
                MergeNsInfo(nsInfo, aliasNsInfo);
            }
        }

        private void CheckDocument(NsInfo nsInfo)
        {
            var document = nsInfo.ModuleMember as DOM.Document;
            if (document != null)
            {
                if (Path.GetExtension(document.Module.FileName) == ".mlx")
                    CheckDocumentElement(document);
            }
        }

        /// <summary>
        /// Checks if XML document has one root element
        /// </summary>
        /// <param name="document"></param>
        private void CheckDocumentElement(DOM.Document document)
        {
            int rootElementCount = 0;
            foreach (var entity in document.Entities)
            {
                if (entity is DOM.Element) rootElementCount++;

                if (entity is DOM.Alias) rootElementCount += CalcNumOfRootElements(entity as DOM.Alias, null);

                if (rootElementCount > 1)
                {
                    _context.Errors.Add(CompilerErrorFactory.DocumentMustHaveOneRootElement(document, document.Module.FileName, " only"));
                    break;
                }
            }
            if (rootElementCount == 0)
            {
                _context.Errors.Add(CompilerErrorFactory.DocumentMustHaveOneRootElement(document, document.Module.FileName, " at least"));
            }


        }

        private int CalcNumOfRootElements(DOM.Alias alias, List<DOM.AliasDefinition> aliasList)
        {
            int result = 0;
            var aliasDef = LookupAliasDef(alias);
            if (aliasDef == null) return 0;

            //Checking if there is circular reference to prevent infinite loop.
            if (aliasList == null) aliasList = new List<DOM.AliasDefinition>();

            if (aliasList.Any(n => n == aliasDef)) return 0;

            aliasList.Add(aliasDef);

            foreach (var entity in aliasDef.Entities)
            {
                if (entity is DOM.Element) result++;
                if (entity is DOM.Alias) result += CalcNumOfRootElements(entity as DOM.Alias, aliasList);
            }

            return result;
        }

        private void MergeNsInfo(NsInfo destNsInfo, NsInfo nsInfo)
        {
            foreach(var ns in nsInfo.Namespaces)
            {
                var uri = ns.Value;
                var destNs = destNsInfo.Namespaces.FirstOrDefault(n => n.Value == uri);

                if (destNs != null) continue;

                var prefix = FindFreePrefix(destNs.Name, destNsInfo.Namespaces);

                destNsInfo.Namespaces.Add(new DOM.Namespace() { Name = prefix, Value = ns.Value });
            }
        }

        private string FindFreePrefix(string name, List<DOM.Namespace> namespaces)
        {
            var i = 1;
            while (namespaces.Any(n => n.Name == name))
            {
                name = name + i++.ToString();
            }

            return name;
        }

        private NsInfo ResolveAliasInDocument(DOM.Alias alias, NsInfo documentNsInfo)
        {
            //Finding AliasDef
            DOM.AliasDefinition aliasDef = null;
            aliasDef = LookupAliasDef(alias);
            if (aliasDef == null)
            {
                //Report Error
                _context.Errors.Add(CompilerErrorFactory.AliasIsNotDefined(alias, (documentNsInfo.ModuleMember as DOM.Document).Module.FileName));
                return null;
            }
            return ResolveAliasesInAliasDefinition(aliasDef);
        }

        private DOM.AliasDefinition LookupAliasDef(DOM.Alias alias)
        {
            DOM.AliasDefinition result = null;
            _context.AliasDefinitions.TryGetValue(alias.Name, out result);
            return result;
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
                foreach (var info in _aliasStack)
                {
                    _context.Errors.Add(CompilerErrorFactory.AliasDefHasCircularReference(info));
                    if (info == aliasDefNsInfo) break;
                }                
                return aliasDefNsInfo;
            }

            _aliasStack.Push(aliasDefNsInfo);

            foreach (var alias in aliasDefNsInfo.Aliases)
            {
                NsInfo aliasNsInfo = ResolveAliasInAliasDefinition(alias, aliasDefNsInfo);
                if (aliasNsInfo == null) continue;
                MergeNsInfo(aliasDefNsInfo, aliasNsInfo);
            }

            _aliasStack.Pop();

            aliasDefNsInfo.AliasesResolved = true;

            return aliasDefNsInfo;
        }

        private NsInfo ResolveAliasInAliasDefinition(DOM.Alias alias, NsInfo aliasDefNsInfo)
        {
            //Finding AliasDef
            DOM.AliasDefinition aliasDef = null;
            _context.AliasDefinitions.TryGetValue(alias.Name, out aliasDef);
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
                _currentModuleMemberNsInfo = result;
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
            _currentModuleMemberNsInfo = new NsInfo(_currentModuleMember);
            ModuleMembersNsInfo.Add(_currentModuleMemberNsInfo);
        }

        public void ExitDocument(DOM.Antlr.Document node)
        {
            //Checking if the document's name is unique
            var sameNameDocuments = ModuleMembersNsInfo.FindAll(n => (n.ModuleMember is DOM.Document && (n.ModuleMember as DOM.Document).Name == node.Name));
            if (sameNameDocuments.Count > 1)
            {
                if (sameNameDocuments.Count == 2)
                {
                    //Reporting error for 2 documents (existing and new)
                    var prevDoc = sameNameDocuments[0].ModuleMember as DOM.Document;
                    _context.Errors.Add(CompilerErrorFactory.DuplicateDocumentName(prevDoc, prevDoc.Module.FileName));
                }
                _context.Errors.Add(CompilerErrorFactory.DuplicateDocumentName(node, _currentModule.FileName));

            }
        }


        public void EnterAliasDef(DOM.Antlr.AliasDefinition node)
        {
            _currentModuleMember = node;
            _currentModuleMemberNsInfo = new NsInfo(_currentModuleMember);
            ModuleMembersNsInfo.Add(_currentModuleMemberNsInfo);

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
        /// if the namespace is not added yet
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

                var foundNs = CurrentModuleMemberNsInfo.Namespaces.FirstOrDefault(n => n.Value == ns.Value);

                if (foundNs == null)
                    CurrentModuleMemberNsInfo.Namespaces.Add(ns);
                else if (foundNs.Name != nsPrefix) (node as INsNode).NsPrefix = foundNs.Name;

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
            //Looking up in the ModuleMember (Document/AliasDef)
            if ((ns = _currentModuleMember.Namespaces.FirstOrDefault(n => n.Name == nsPrefix)) != null)
                return ns;

            //Looking up in the ModuleMember
            if ((ns = _currentModule.Namespaces.FirstOrDefault(n => n.Name == nsPrefix)) != null)
            {
                //Checking if this namespace can be replaced by ns from ModuleMember because it has same URI
                DOM.Namespace ns2 = null;
                if ((ns2 = _currentModuleMember.Namespaces.FirstOrDefault(n => n.Value == ns.Value)) != null)
                    return ns2;

                return ns;
            }
                

            return null;
        }

        public void GetPrefixAndNs(INsNode node, DOM.Document document, DOM.AliasDefinition aliasDef, out string prefix, out string ns)
        {
            prefix = null;
            ns = null;

            if (node.NsPrefix == null) return;

            var targetNsInfo = ModuleMembersNsInfo.FirstOrDefault(n => n.ModuleMember == document);

            if(aliasDef == null)
            {//Resolving based on document's NsInfo

                var domNamespace = targetNsInfo.Namespaces.FirstOrDefault(n => n.Name == node.NsPrefix);
                if (domNamespace != null)
                {
                    prefix = domNamespace.Name;
                    ns = domNamespace.Value;
                }
                else
                {
                    //Prefix is defined in Module. Resolving using module namespaces
                    //Finding uri first
                    var moduleNamespace = document.Module.Namespaces.FirstOrDefault(n => n.Name == node.NsPrefix);
                    if (moduleNamespace!= null)
                    {
                        ns = moduleNamespace.Value;

                        //Finding effective prefix in the Document Namespace
                        domNamespace = targetNsInfo.Namespaces.FirstOrDefault(n => n.Value == moduleNamespace.Value);
                        prefix = domNamespace.Name;
                    }
                }
            }
            else
            {
                //Resolving ns first using aliasDef context NsInfo
                var contextNsInfo = ModuleMembersNsInfo.FirstOrDefault(n => n.ModuleMember == aliasDef);
                var domNamespace = contextNsInfo.Namespaces.FirstOrDefault(n => n.Name == node.NsPrefix);
                
                if (domNamespace == null)
                {
                    //Prefix was defined in the module. Looking up in the module.
                    var moduleNamespace = aliasDef.Module.Namespaces.FirstOrDefault(n => n.Name == node.NsPrefix);
                    if (moduleNamespace != null)
                        ns = moduleNamespace.Value;
                }
                else
                {
                    ns = domNamespace.Value;
                }
                //Resolving prefix using Document's NsInfo
                if(ns != null)
                {
                    var ns1 = ns;
                    prefix = targetNsInfo.Namespaces.First(n => n.Value == ns1).Name;
                }
            }
        }
    }
}
