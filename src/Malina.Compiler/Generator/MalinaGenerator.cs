using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Malina.DOM;
using Malina.Parser;
using ValueType = Malina.DOM.ValueType;

namespace Malina.Compiler.Generator
{
    public class MalinaGenerator : MalinaDepthFirstVisitor
    {

        private Stack<AliasContext> _aliasContext;
        protected CompilerContext _context;
        protected Document _currentDocument;

        protected Stack<AliasContext> AliasContext
        {
            get
            {
                if (_aliasContext != null) return _aliasContext;

                _aliasContext = new Stack<AliasContext>();
                _aliasContext.Push(null);

                return _aliasContext;
            }
        }

        public MalinaGenerator(CompilerContext context)
        {
            _context = context;
        }


        protected string ResolveNodeValue(DOM.Antlr.IValueNode node)
        {
            if (((IValueNode)node).ValueType != ValueType.SingleQuotedString)
                return ((IValueNode)node).Value;

            var sb = new StringBuilder();
            foreach (var item in node.InterpolationItems)
            {
                var alias = item as Alias;
                if (alias != null)
                {
                    sb.Append(ResolveValueAlias(alias));
                    continue;
                }
                var token = item as CommonToken;
                if (token == null) continue;

                sb.Append(token.Type == MalinaLexer.SQS_EOL ? ResolveSqsEol(token, node.ValueIndent) : token.Text);
            }
            return sb.ToString();
        }

        protected string ResolveValueAlias(Alias alias)
        {
            var aliasDef = _context.NamespaceResolver.GetAliasDefinition(alias.Name);

            return aliasDef.ObjectValue == null ? aliasDef.Value : ResolveObjectValue(aliasDef.ObjectValue);
        }


        private string ResolveObjectValue(object objectValue)
        {
            var value = objectValue as Parameter;
            if (value != null)
            {
                return ResolveValueParameter(value);
            }

            var alias = objectValue as Alias;
            if (alias != null)
            {
                return ResolveValueAlias(alias);
            }
            return null;
        }


        protected string ResolveValueParameter(Parameter parameter)
        {
            var aliasContext = AliasContext.Peek();
            var argument = aliasContext.Alias.Arguments.FirstOrDefault(a => a.Name == parameter.Name);
            if (argument != null)
            {
                if (argument.ObjectValue != null)
                {
                    return ResolveObjectValue(argument.ObjectValue);
                }
                return argument.Value;
            }

            //if argument is not found lookup default value in the Alias Definition
            var paramDef = (aliasContext.AliasDefinition as DOM.Antlr.AliasDefinition)?.Parameters.First(p => p.Name == parameter.Name);

            //If parameteres default value is Parameter or Alias then resolve it
            if (paramDef?.ObjectValue != null)
            {
                return ResolveObjectValue(paramDef.ObjectValue);

            }

            return paramDef?.Value;
        }

        protected void ResolveAttributes(IEnumerable<DOM.Attribute> attributes, IEnumerable<Entity> entities)
        {
            Visit(attributes);
            ResolveAttributes(entities);
        }


        /// <summary>
        /// Go through all entities and resolve attributes for the current node.
        /// </summary>
        /// <param name="entities">List of entities. Looking for alias or parameter because they potentially can hold the attributes.</param>
        private void ResolveAttributes(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity is Alias)
                {
                    ResolveAttributesInAlias(entity as Alias);
                }
                else if (entity is Parameter)
                {
                    ResolveAttributesInParameter(entity as Parameter);
                }
            }
        }



        private void ResolveAttributesInParameter(Parameter parameter)
        {
            var aliasContext = AliasContext.Peek();
            var argument = aliasContext.Alias.Arguments.FirstOrDefault(a => a.Name == parameter.Name);
            if (argument != null)
            {
                //Resolve using argument's block
                ResolveAttributes(argument.Attributes, argument.Entities);
            }
            else
            {
                //Resolve using parameter's default block
                ResolveAttributes(parameter.Attributes, parameter.Entities);
            }
        }

        private void ResolveAttributesInAlias(Alias alias)
        {
            var aliasDef = _context.NamespaceResolver.GetAliasDefinition(alias.Name);
            AliasContext.Push(new AliasContext() { AliasDefinition = aliasDef, Alias = alias, AliasNsInfo = GetContextNsInfo() });
            ResolveAttributes(aliasDef.Attributes, aliasDef.Entities);
            AliasContext.Pop();
        }


        protected NsInfo GetContextNsInfo()
        {
            if (AliasContext.Peek() == null)
            {
                return _context.NamespaceResolver.GetNsInfo(_currentDocument);
            }

            return _context.NamespaceResolver.GetNsInfo(AliasContext.Peek().AliasDefinition);
        }

        private static string ResolveSqsEol(IToken token, int valueIndent)
        {
            var sb = new StringBuilder();
            var value = token.Text;
            var lines = value.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var first = true;
            foreach (var item in lines)
            {
                //Skip first one
                if (first)
                {
                    first = false;
                    continue;
                }

                //Removing indents
                sb.AppendLine();
                if (item.Length > valueIndent)
                    sb.Append(item.Substring(valueIndent));
            }

            return sb.ToString();
        }



    }
}
