#region license
// Copyright © 2016, 2017 Maxim Trushin (dba Syntactik, trushin@gmail.com, maxim.trushin@syntactik.com)
//
// This file is part of Malina.
// Malina is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// Malina is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Malina.  If not, see <http://www.gnu.org/licenses/>.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Malina.DOM;
using System.IO;
using AliasDefinition = Malina.DOM.Antlr.AliasDefinition;
using ValueType = Malina.DOM.ValueType;

namespace Malina.Compiler
{
    public class NamespaceResolver
    {
        private List<NsInfo> _moduleMembersNsInfo;
        private ModuleMember _currentModuleMember;
        private Module _currentModule;
        private readonly CompilerContext _context;
        private readonly Stack<NsInfo> _aliasDefStack;

        public NamespaceResolver(CompilerContext context)
        {
            _context = context;
            _aliasDefStack = new Stack<NsInfo>();
        }

        /// <summary>
        /// NsInfo for all Module Members (Documents and AliasDef)
        /// </summary>
        public List<NsInfo> ModuleMembersNsInfo => _moduleMembersNsInfo ?? (_moduleMembersNsInfo = new List<NsInfo>());

        //This method is called from ProcessAliasesAndNamespaces step after the all Nodes are visited.
        public void ResolveAliasesAndDoChecks()
        {
            foreach(var nsInfo in ModuleMembersNsInfo)
            {
                CheckModuleMember(nsInfo);

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

        internal void ProcessParameter(Parameter node)
        {
            if(_currentModuleMember is Document)
            {
                _context.AddError(CompilerErrorFactory.ParametersCantBeDeclaredInDocuments(node, _currentModule.FileName));
            }
            else
            {
                ((AliasDefinition) _currentModuleMember).Parameters.Add(node);
                if (node.Name == "_")
                {
                    if (!node.IsValueNode)
                        ((AliasDefinition) _currentModuleMember).HasDefaultBlockParameter = true;
                    else
                        ((AliasDefinition)_currentModuleMember).HasDefaultValueParameter = true;
                }
            }
        }

        public NsInfo GetNsInfo(ModuleMember document)
        {
            return ModuleMembersNsInfo.FirstOrDefault(n => n.ModuleMember == document);
        }

        private void CheckModuleMember(NsInfo nsInfo)
        {
            var document = nsInfo.ModuleMember as Document;

            if (document == null)
            {
                CheckAliasDef((AliasDefinition)nsInfo.ModuleMember);
                return;
            }

            if (Path.GetExtension(document.Module.FileName) == ".mlx")
                CheckDocumentElement(document);
        }

        private void CheckAliasDef(AliasDefinition aliasDef)
        {
            if (!aliasDef.HasDefaultBlockParameter && !aliasDef.HasDefaultValueParameter) return;

            var hasNonDefaultParameter = aliasDef.Parameters.Any(p => p.Name != "_");
            if (!hasNonDefaultParameter) return;

            foreach (var param in aliasDef.Parameters)
            {
                if (param.Name == "_")
                {
                    _context.AddError(CompilerErrorFactory.DefaultParameterMustBeOnly(param, aliasDef.Module.FileName));
                }
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

                if (entity is DOM.Antlr.Alias) rootElementCount += CalcNumOfRootElements(entity as DOM.Antlr.Alias, null);

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

        public AliasDefinition GetAliasDefinition(string name)
        {
            NsInfo resultInfo = ModuleMembersNsInfo.FirstOrDefault(a => (a.ModuleMember is DOM.AliasDefinition) && a.ModuleMember.Name == name);
            return (AliasDefinition) resultInfo?.ModuleMember;
        }

        private int CalcNumOfRootElements(DOM.Antlr.Alias alias, List<DOM.AliasDefinition> aliasList)
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
                var aliasEntity = entity as DOM.Antlr.Alias;
                if (aliasEntity != null) result += CalcNumOfRootElements(aliasEntity, aliasList);
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

        private NsInfo ResolveAliasInModuleMember(DOM.Antlr.Alias alias, NsInfo memberNsInfo)
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
            CheckCompatibilityWithParameters(alias, aliasDef, documentNsInfo);
            CheckForUnexpectedArguments(alias, aliasDef, documentNsInfo);
        }

        private void CheckForUnexpectedArguments(Alias alias, AliasDefinition aliasDef, NsInfo documentNsInfo)
        {
            foreach (var argument in alias.Arguments)
            {
                if (aliasDef.Parameters.All(p => p.Name != argument.Name)) _context.AddError(CompilerErrorFactory.UnexpectedArgument(argument, documentNsInfo.ModuleMember.Module.FileName));
            }

            if (!aliasDef.HasDefaultBlockParameter && alias.Entities.Count > 0)
            {
                _context.AddError(CompilerErrorFactory.UnexpectedDefaultBlockArgument(alias.Entities[0],
                            documentNsInfo.ModuleMember.Module.FileName));
            }

            if (!aliasDef.HasDefaultValueParameter && alias.HasValue())
            {
                _context.AddError(CompilerErrorFactory.UnexpectedDefaultValueArgument(alias,
                            documentNsInfo.ModuleMember.Module.FileName));
            }
        }

        private void CheckCompatibilityWithParameters(Alias alias, AliasDefinition aliasDef, NsInfo documentNsInfo)
        {
            foreach (var parameter in aliasDef.Parameters)
            {
                if (parameter.Name == "_") //Default parameter
                {
                    if (!parameter.IsValueNode)
                    {
                        if (alias.Entities.Count == 0 && alias.ValueType != ValueType.EmptyObject)
                            _context.AddError(CompilerErrorFactory.DefaultBlockArgumentIsMissing(alias,
                                documentNsInfo.ModuleMember.Module.FileName));
                    }
                    else
                    {
                        if (parameter.HasValue()) continue; //if parameter has default value then skip check

                        if (!alias.HasValue())
                        {
                            _context.AddError(CompilerErrorFactory.DefaultValueArgumentIsMissing(alias,
                                documentNsInfo.ModuleMember.Module.FileName));
                        }
                    }


                    continue;
                }

                //Non default parameter
                var argument = alias.Arguments.FirstOrDefault(a => a.Name == parameter.Name);
                if (argument == null)
                {
                    //Report Error if argument is missing and there is no default value for the parameter
                    if (parameter.Value == null && parameter.Entities.Count == 0 && parameter.ValueType != ValueType.EmptyObject)
                        _context.AddError(CompilerErrorFactory.ArgumentIsMissing(alias, parameter.Name,
                            documentNsInfo.ModuleMember.Module.FileName));

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

        private DOM.Antlr.AliasDefinition LookupAliasDef(DOM.Antlr.Alias alias)
        {
            if (alias.AliasDefinition != null)
                return alias.AliasDefinition == AliasDefinition.Undefined ? null : alias.AliasDefinition;

            var result = GetAliasDefinition(alias.Name);

            alias.AliasDefinition = result ?? AliasDefinition.Undefined;
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

            if (_aliasDefStack.Any(n => n == aliasDefNsInfo))
            {
                //Report Error
                foreach (var info in _aliasDefStack)
                {
                    _context.AddError(CompilerErrorFactory.AliasDefHasCircularReference(info));
                    ((DOM.Antlr.AliasDefinition) info.ModuleMember).HasCircularReference = true;
                    if (info == aliasDefNsInfo) break;
                }                
                return aliasDefNsInfo;
            }

            _aliasDefStack.Push(aliasDefNsInfo);

            foreach (var alias in aliasDefNsInfo.Aliases)
            {
                NsInfo aliasNsInfo = ResolveAliasInAliasDefinition(alias, aliasDefNsInfo);
                if (aliasNsInfo == null) continue;
                MergeNsInfo(aliasDefNsInfo, aliasNsInfo);
            }

            _aliasDefStack.Pop();

            aliasDefNsInfo.AliasesResolved = true;

            return aliasDefNsInfo;
        }

        private NsInfo ResolveAliasInAliasDefinition(DOM.Alias alias, NsInfo aliasDefNsInfo)
        {
            //Finding AliasDef
            var aliasDef = GetAliasDefinition(alias.Name);
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

            //Looking up in the Module
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

        public void GetPrefixAndNs(INsNode node, DOM.Document document, Func<Scope> getScope, out string prefix,
            out string ns)
        {
            prefix = null;
            ns = null;

            if (node.NsPrefix == null)
            {
                var scope = getScope();
                if (scope == null) return;
                node.NsPrefix = scope.NsPrefix;
            }

            //Getting namespace info for the generated document.
            var targetNsInfo = ModuleMembersNsInfo.First(n => n.ModuleMember == document);
            var moduleMember = GetModuleMember((Node) node);
            var member = moduleMember as ModuleMember;
            if (member != null)
            {
                //Resolving ns first using aliasDef context NsInfo
                var contextNsInfo = ModuleMembersNsInfo.First(n => n.ModuleMember == moduleMember);
                var domNamespace = contextNsInfo.Namespaces.FirstOrDefault(n => n.Name == node.NsPrefix);

                if (domNamespace == null)
                {
                    //Prefix was defined in the module. Looking up in the module.
                    var moduleNamespace = member.Module.Namespaces.FirstOrDefault(n => n.Name == node.NsPrefix);
                    if (moduleNamespace != null)
                        ns = moduleNamespace.Value;
                }
                else
                {
                    ns = domNamespace.Value;
                }
                //Resolving prefix using Document's NsInfo
                if (ns != null)
                {
                    var ns1 = ns;
                    prefix = targetNsInfo.Namespaces.First(n => n.Value == ns1).Name;
                }
            }
        }

        private Node GetModuleMember(Node node)
        {
            while (!(node.Parent is ModuleMember) && !(node.Parent is Module)) node = node.Parent;
            return (ModuleMember) node.Parent;
        }
    }
}
