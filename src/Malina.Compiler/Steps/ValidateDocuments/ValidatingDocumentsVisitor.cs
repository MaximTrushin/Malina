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
using Malina.Compiler.Generator;
using Malina.DOM;
using Attribute = Malina.DOM.Attribute;

namespace Malina.Compiler.Steps
{
    public class ValidatingDocumentsVisitor: AliasResolvingVisitor
    {   
        private bool _blockStart;
        private Stack<JsonGenerator.BlockState> _blockState;
        private DOM.Antlr.Module _currentModule;

        public ValidatingDocumentsVisitor(CompilerContext context):base(context)
        {
        }

        public override void OnModule(Module node)
        {
            _currentModule = (DOM.Antlr.Module) node;

            base.OnModule(node);

        }

        public override void OnDocument(Document node)
        {
            _blockStart = true;
            _blockState = new Stack<JsonGenerator.BlockState>();

            base.OnDocument(node);
        }

        public override void OnElement(Element node)
        {
            CheckBlockIntegrity(node);

            CheckArrayItem(node);

            if (HasValue(node)) return; //This is a value element. It doesn't have block. Don't continue to validate element's block.

            _blockStart = true;
            var prevBlockStateCount = _blockState.Count;

            base.OnElement(node);

            _blockStart = false;

            if (_blockState.Count > prevBlockStateCount)
            {
                _blockState.Pop();
            }
        }

        private void CheckArrayItem(Element node)
        {
            if (_currentModule.TargetFormat != DOM.Antlr.Module.TargetFormats.Xml) return;
            if (string.IsNullOrEmpty(node.Name) && !node.IsValueNode)
            {
                _context.AddError(CompilerErrorFactory.CantDefineArrayItemInXmlDocument(node, _currentModule.FileName));
            }
        }

        public override void OnAttribute(Attribute node)
        {
            CheckBlockIntegrity(node);
        }

        public override void OnArgument(Argument argument)
        {
            CheckArgumentIntegrity(argument);
            base.OnArgument(argument);
        }

        private void CheckArgumentIntegrity(Argument argument)
        {
            if (! (argument.Parent is Alias))
                _context.AddError(CompilerErrorFactory.ArgumentMustBeDefinedInAlias(argument, _currentModule.FileName));
        }

        private void CheckBlockIntegrity(Node node)
        {
            if (!_blockStart)
            {
                var blockState = _blockState.Peek();
                if (blockState == JsonGenerator.BlockState.Array)
                {
                    if (!string.IsNullOrEmpty(node.Name))
                    {
                        ReportErrorForEachNodeInAliasContext(
                            n => CompilerErrorFactory.ArrayItemIsExpected(n, _currentModule.FileName));
                        _context.AddError(CompilerErrorFactory.ArrayItemIsExpected(node, _currentModule.FileName));
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(node.Name)) return;

                    if(_currentModule.TargetFormat == DOM.Antlr.Module.TargetFormats.Xml && ((IValueNode)node).IsValueNode ) return; 

                    ReportErrorForEachNodeInAliasContext(
                        n => CompilerErrorFactory.PropertyIsExpected(n, _currentModule.FileName));
                    _context.AddError(CompilerErrorFactory.PropertyIsExpected(node, _currentModule.FileName));
                }

                return;
            }

            //This element is the first element of the block. It decides if the block is array or object
            _blockState.Push(string.IsNullOrEmpty(node.Name)
                ? JsonGenerator.BlockState.Array
                : JsonGenerator.BlockState.Object);

            _blockStart = false;
        }

        private void ReportErrorForEachNodeInAliasContext(Func<Node, CompilerError> func)
        {
            foreach (var item in AliasContext)
            {
                if (item != null)
                {
                    _context.AddError(func(item.Alias));
                }
            }
        }

        private static bool HasValue(IValueNode node)
        {
            object value = node.ObjectValue as Parameter;
            if (value != null)
            {
                return true;
            }

            value = node.ObjectValue as Alias;
            if (value != null)
            {
                return true;
            }

            return node.Value != null;
        }
    }
}
