using System;
using System.Collections.Generic;
using System.Linq;
using Malina.DOM;
using System.IO;
using AliasDefinition = Malina.DOM.Antlr.AliasDefinition;

namespace Malina.Compiler
{
    public class NamespaceResolver
    {
        private List<NsInfo> _moduleMembersNsInfo;
        private ModuleMember _currentModuleMember;
        private Module _currentModule;
        private readonly CompilerContext _context;
        private readonly Stack<NsInfo> _aliasStack;

        public NamespaceResolver(CompilerContext context)
        {
            _context = context;
            _aliasStack = new Stack<NsInfo>();
        }

        /// <summary>
        /// NsInfo for all Module Members (Documents and AliasDef)
        /// </summary>
        private List<NsInfo> ModuleMembersNsInfo => _moduleMembersNsInfo ?? (_moduleMembersNsInfo = new List<NsInfo>());

        //This method is called from ProcessAliasesAndNamespaces step after the all Nodes are visited.
        public void ResolveAliasesAndDoChecks()
        {
            foreach(var nsInfo in ModuleMembersNsInfo)
            {
                CheckDocument(nsInfo);

                ResolveAliasesInModuleMember(nsInfo);
            }
        }

        private void ResolveAliasesInModuleMember(NsInfo nsInfo)
        {
            foreach (var alias in nsInfo.Aliases)
            {
                NsInfo aliasNsInfo = ResolveAliasInModuleMember(alias, nsInfo);
                if (aliasNsInfo == null) continue;
                MergeNsInfo(nsInfo, aliasNsInfo);
            }
        }

        internal void ProcessParameter(DOM.Antlr.Parameter node)
        {
            if(_currentModuleMember is Document)
            {
                _context.AddError(CompilerErrorFactory.ParametersCantBeDeclaredInDocuments(node, _currentModule.FileName));
            }
            else
            {
                ((AliasDefinition) _currentModuleMember).Parameters.Add(node);
            }
        }

        public NsInfo GetNsInfo(ModuleMember document)
        {
            return ModuleMembersNsInfo.FirstOrDefault(n => n.ModuleMember == document);
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
                    _context.AddError(CompilerErrorFactory.DocumentMustHaveOneRootElement(document, document.Module.FileName, " only"));
                    break;
                }
            }
            if (rootElementCount == 0)
            {
                _context.AddError(CompilerErrorFactory.DocumentMustHaveOneRootElement(document, document.Module.FileName, " at least"));
            }
        }

        public DOM.AliasDefinition GetAliasDefinition(string name)
        {
            NsInfo resultInfo = ModuleMembersNsInfo.FirstOrDefault(a => (a.ModuleMember is DOM.AliasDefinition) && a.ModuleMember.Name == name);
            return (DOM.AliasDefinition) resultInfo?.ModuleMember;
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

                var prefix = FindFreePrefix(ns.Name, destNsInfo.Namespaces);

                destNsInfo.Namespaces.Add(new DOM.Namespace() { Name = prefix, Value = ns.Value });
            }
        }

        private string FindFreePrefix(string name, List<DOM.Namespace> namespaces)
        {
            var i = 1;
            while (namespaces.Any(n => n.Name == name))
            {
                name = name + i++;
            }

            return name;
        }

        private NsInfo ResolveAliasInModuleMember(DOM.Alias alias, NsInfo memberNsInfo)
        {
            //Finding AliasDef
            var aliasDef = LookupAliasDef(alias);

            if (aliasDef == null)
            {
                //Report Error if alias is not defined
                _context.AddError(CompilerErrorFactory.AliasIsNotDefined(alias, memberNsInfo.ModuleMember.Module.FileName));
                return null;
            }

            if (aliasDef.IsValueNode != alias.IsValueNode)
            {
                _context.AddError(aliasDef.IsValueNode
                    ? CompilerErrorFactory.CantUseValueAliasInTheBlock(alias, memberNsInfo.ModuleMember.Module.FileName)
                    : CompilerErrorFactory.CantUseBlockAliasAsValue(alias, memberNsInfo.ModuleMember.Module.FileName));
            }


            CheckAliasArguments(alias, aliasDef, memberNsInfo);

            return ResolveAliasesInAliasDefinition(aliasDef);
        }

        private void CheckAliasArguments(DOM.Alias alias, DOM.Antlr.AliasDefinition aliasDef, NsInfo documentNsInfo)
        {            
            foreach (var parameter in aliasDef.Parameters)
            {
                DOM.Argument argument = alias.Arguments.FirstOrDefault(a => a.Name == parameter.Name);
                if (argument == null)
                {
                    //Report Error if argument is missing and there is no default value for the parameter
                    if (parameter.Value == null && parameter.Attributes.Count == 0 && parameter.Entities.Count == 0) _context.AddError(CompilerErrorFactory.ArgumentIsMissing(alias, parameter.Name, documentNsInfo.ModuleMember.Module.FileName));
                    continue;
                }

                //Report error if type of argument (value/block) mismatch the type of parameter
                if (argument.IsValueNode != parameter.IsValueNode)
                {
                    _context.AddError(parameter.IsValueNode
                        ? CompilerErrorFactory.ValueArgumentIsExpected(argument,
                            documentNsInfo.ModuleMember.Module.FileName)
                        : CompilerErrorFactory.BlockArgumentIsExpected(argument,
                            documentNsInfo.ModuleMember.Module.FileName));
                }

            }
        }

        private DOM.Antlr.AliasDefinition LookupAliasDef(DOM.Alias alias)
        {
            var result = (DOM.Antlr.AliasDefinition)_context.NamespaceResolver.GetAliasDefinition(alias.Name);
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
                    _context.AddError(CompilerErrorFactory.AliasDefHasCircularReference(info));
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
            var aliasDef = _context.NamespaceResolver.GetAliasDefinition(alias.Name);
            if (aliasDef == null)
            {
                //Report Error
                _context.AddError(CompilerErrorFactory.AliasIsNotDefined(alias, aliasDefNsInfo.ModuleMember.Module.FileName));
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
            CheckDuplicateArguments(node);
        }

        /// <summary>
        /// Adds Alias to the Namespace Info of the current Module Member.
        /// </summary>
        /// <param name="node"></param>
        public void AddAlias(DOM.Antlr.Alias node)
        {
            CurrentModuleMemberNsInfo.Aliases.Add(node);
        }

        private void CheckDuplicateArguments(DOM.Antlr.Alias alias)
        {
            var dups = alias.Arguments.GroupBy(a => a.Name).Where(g => g.Count() > 1).SelectMany(g => g).ToList();
            dups.ForEach(a => _context.AddError(CompilerErrorFactory.DuplicateArgumentName(a, _currentModule.FileName)));


        }

        public void EnterDocument(DOM.Antlr.Document node)
        {
            _currentModuleMember = node;
            _currentModuleMemberNsInfo = new NsInfo(_currentModuleMember);
            ModuleMembersNsInfo.Add(_currentModuleMemberNsInfo);
        }

        public void ExitDocument(DOM.Antlr.Document node)
        {
            //Checking if the document's name is unique per format (json/xml)
            var sameNameDocuments = ModuleMembersNsInfo.FindAll(n => (n.ModuleMember is DOM.Document && ((DOM.Document) n.ModuleMember).Name == node.Name) 
                && ((DOM.Antlr.Module)node.Module).TargetFormat == ((DOM.Antlr.Module)((DOM.Document)n.ModuleMember).Module).TargetFormat
             );
            if (sameNameDocuments.Count > 1)
            {
                if (sameNameDocuments.Count == 2)
                {
                    //Reporting error for 2 documents (existing and new)
                    var prevDoc = (DOM.Document) sameNameDocuments[0].ModuleMember;
                    _context.AddError(CompilerErrorFactory.DuplicateDocumentName(prevDoc, prevDoc.Module.FileName));
                }
                _context.AddError(CompilerErrorFactory.DuplicateDocumentName(node, _currentModule.FileName));

            }
        }


        public void ExitAliasDef(DOM.Antlr.AliasDefinition node)
        {
            //Checking if the alias definition name is unique
            var sameNameAliasDef = ModuleMembersNsInfo.FindAll(n => (n.ModuleMember is DOM.AliasDefinition && ((DOM.AliasDefinition) n.ModuleMember).Name == node.Name));
            if (sameNameAliasDef.Count > 1)
            {
                if (sameNameAliasDef.Count == 2)
                {
                    //Reporting error for 2 documents (existing and new)
                    var prevAliasDef = (DOM.AliasDefinition) sameNameAliasDef[0].ModuleMember;
                    _context.AddError(CompilerErrorFactory.DuplicateAliasDefName(prevAliasDef, prevAliasDef.Module.FileName));
                }
                _context.AddError(CompilerErrorFactory.DuplicateAliasDefName(node, _currentModule.FileName));

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
            var nsPrefix = ((INsNode) node).NsPrefix;

            if (nsPrefix != null && !nsPrefix.StartsWith("xml", StringComparison.OrdinalIgnoreCase))
            {
                var ns = LookupNamespace(nsPrefix);
                if (ns == null)
                {
                    _context.AddError(CompilerErrorFactory.NsPrefixNotDefined(node, _currentModule.FileName));
                    return;
                }

                var foundNs = CurrentModuleMemberNsInfo.Namespaces.FirstOrDefault(n => n.Value == ns.Value);

                if (foundNs == null)
                    CurrentModuleMemberNsInfo.Namespaces.Add(ns);
                else if (foundNs.Name != nsPrefix) ((INsNode) node).NsPrefix = foundNs.Name;

            }
        }

        /// <summary>
        /// Looks up Namespace with the specified Prefix in the current ModuleMember first and then in the Module
        /// </summary>
        /// <param name="nsPrefix"></param>
        /// <returns></returns>
        private DOM.Namespace LookupNamespace(string nsPrefix)
        {
            DOM.Namespace ns;
            //Looking up in the ModuleMember (Document/AliasDef)
            if ((ns = _currentModuleMember.Namespaces.FirstOrDefault(n => n.Name == nsPrefix)) != null)
                return ns;

            //Looking up in the ModuleMember
            if ((ns = _currentModule.Namespaces.FirstOrDefault(n => n.Name == nsPrefix)) != null)
            {
                //Checking if this namespace can be replaced by ns from ModuleMember because it has same URI
                DOM.Namespace ns2;
                if ((ns2 = _currentModuleMember.Namespaces.FirstOrDefault(n => n.Value == ns.Value)) != null)
                    return ns2;

                return ns;
            }

            return null;
        }

        public void GetPrefixAndNs(INsNode node, DOM.Document document, Func<DOM.AliasDefinition> getAliasDef, out string prefix, out string ns)
        {
            prefix = null;
            ns = null;

            if (node.NsPrefix == null) return;//No prefix no cry


            //Getting namespace info for the generated document.
            var targetNsInfo = ModuleMembersNsInfo.First(n => n.ModuleMember == document);

            var aliasDef = getAliasDef();

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
                        domNamespace = targetNsInfo.Namespaces.First(n => n.Value == moduleNamespace.Value);
                        prefix = domNamespace.Name;
                    }
                }
            }
            else
            {
                //Resolving ns first using aliasDef context NsInfo
                var contextNsInfo = ModuleMembersNsInfo.First(n => n.ModuleMember == aliasDef);
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
