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
using System.Collections.Generic;

namespace Malina.DOM
{
    public class MalinaDepthFirstVisitor : IDomVisitor
    {

        public virtual void OnAlias(Alias node)
        {
            Visit(node.Arguments);
            Visit(node.Entities);
        }

        public virtual void OnAliasDefinition(AliasDefinition node)
        {
            Visit(node.Namespaces);
            Visit(node.Entities);
        }

        public virtual void OnArgument(Argument node)
        {
            Visit(node.Entities);
        }

        public virtual void OnAttribute(Attribute node)
        {

        }

        public void OnCompileUnit(CompileUnit node)
        {
            Visit(node.Modules);
        }
        public virtual void OnDocument(Document node)
        {
            Visit(node.Namespaces);
            Visit(node.Entities);
        }

        public virtual void OnElement(Element node)
        {
            Visit(node.Entities);
        }

        public virtual void OnModule(Module node)
        {
            Visit(node.Namespaces);
            Visit(node.Members);
        }

        public virtual void OnNamespace(Namespace node)
        {
        }

        public virtual void OnScope(Scope node)
        {
            Visit(node.Entities);
        }

        protected virtual void OnNode(Node node)
        {
            node.Accept(this);
        }

        public virtual void OnParameter(Parameter node)
        {
            Visit(node.Entities);
        }


        public void Visit(Node node)
        {
            OnNode(node);
        }

        public void Visit<T>(IEnumerable<T> items) where T : Node
        {
            if (items == null) return;
            
            foreach (var node in items)
                OnNode(node);

        }


    }
}
