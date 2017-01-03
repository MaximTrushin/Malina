using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Malina.DOM;
using Malina.Parser;
using Attribute = Malina.DOM.Attribute;
using ValueType = Malina.DOM.ValueType;

namespace Malina.Compiler.Generator
{
    public class AliasResolvingVisitor : MalinaDepthFirstVisitor
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

        public AliasResolvingVisitor(CompilerContext context)
        {
            _context = context;
        }


        protected string ResolveNodeValue(DOM.Antlr.IValueNode node, out ValueType valueType)
        {
            if (((IValueNode) node).ValueType != ValueType.SingleQuotedString)
            {
                valueType = ((IValueNode) node).ValueType;
                return ((IValueNode) node).Value;
            }
            var sb = new StringBuilder();
            foreach (var item in node.InterpolationItems)
            {
                var alias = item as Alias;
                if (alias != null)
                {
                    sb.Append(ResolveValueAlias(alias, out valueType));
                    continue;
                }
                var param = item as Parameter;
                if (param != null)
                {
                    sb.Append(ResolveValueParameter(param, out valueType));
                    continue;
                }
                var token = item as CommonToken;
                if (token == null) continue;
                if (token.Type == MalinaLexer.SQS_ESCAPE)
                {
                    ResolveSqsEscape(token, sb);
                }
                else
                    sb.Append(token.Type == MalinaLexer.SQS_EOL ? ResolveSqsEol(token, node.ValueIndent) : token.Text);
            }

            valueType = ValueType.SingleQuotedString;
            return sb.ToString();
        }

        protected virtual void ResolveSqsEscape(CommonToken token, StringBuilder sb)
        {
            char c = ResolveSqsEscapeChar(token);
            sb.Append(c);
        }

        protected char ResolveSqsEscapeChar(CommonToken token)
        {
            var text = token.Text;

            switch (text)
            {
                case "$$":
                    return '$';

                case "''":
                    return '\'';
            }

            if (text[0] == '$') return ResolveSqsEscapeCode(token);

            //This is EscSeq
            switch (text[1])
            {   //'$'|'b'|'f'|'n'|'r'|'t'|'v'
                case '"':
                case '\'':
                case '\\':
                case '/':
                case '$':
                    return text[1];
                case 'b':
                    return (char)8;
                case 'f':
                    return (char)0xC;
                case 'n':
                    return (char)0xA;
                case 'r':
                    return (char)0xD;
                case 't':
                    return (char)0x9;
                case 'v':
                    return (char)0xB;
                case 'u':
                    return Convert.ToChar(Convert.ToUInt32(text.Substring(2), 16));

            }

            return (char)0;//should never reach this code if lexer works correctly
        }

        protected char ResolveSqsEscapeCode(CommonToken token)
        {
            var code = token.Text.TrimStart('$', '(', '\t', ' ').TrimEnd(')', '\t', ' ');

            if (code[0] != '%') return (char) int.Parse(code);
            return Convert.ToChar(Convert.ToUInt32(code.Substring(1), 16));
        }

        protected string ResolveValueAlias(Alias alias, out ValueType valueType)
        {
            var aliasDef = _context.NamespaceResolver.GetAliasDefinition(alias.Name);

            AliasContext.Push(new AliasContext { AliasDefinition = aliasDef, Alias = alias, AliasNsInfo = GetContextNsInfo() });

            var result = aliasDef.ObjectValue == null ? ResolveNodeValue(aliasDef, out valueType) : ResolveObjectValue(aliasDef.ObjectValue, out valueType);

            AliasContext.Pop();

            return result;
        }


        private string ResolveObjectValue(object objectValue, out ValueType valueType)
        {
            var value = objectValue as Parameter;
            if (value != null)
            {
                return ResolveValueParameter(value, out valueType);
            }

            var alias = objectValue as Alias;
            valueType = ValueType.None;
            return alias != null ? ResolveValueAlias(alias, out valueType) : null;
        }

        protected bool ResolveValue(IValueNode node)
        {
            //Write element's value
            object value = node.ObjectValue as Parameter;
            ValueType valueType;
            if (value != null)
            {
                OnValue(ResolveValueParameter((Parameter)value, out valueType), valueType);
                return true;
            }

            value = node.ObjectValue as Alias;
            if (value != null)
            {
                OnValue(ResolveValueAlias((Alias)value, out valueType), valueType);
                return true;
            }
            if (node.IsValueNode)
            {
                OnValue(ResolveNodeValue((DOM.Antlr.IValueNode)node, out valueType), node.ValueType);
                return true;
            }

            return false;
        }

        public virtual void OnValue(string value, ValueType type)
        {
        }

        protected string ResolveValueParameter(Parameter parameter, out ValueType valueType)
        {
            var aliasContext = AliasContext.Peek();

            if (parameter.Name == "_")
            {
                //Resolving default value parameter
                if (aliasContext.Alias.ObjectValue != null)
                {
                    return ResolveObjectValue(aliasContext.Alias.ObjectValue, out valueType);
                }
                return ResolveNodeValue((DOM.Antlr.IValueNode) aliasContext.Alias, out valueType);
            }


            var argument = aliasContext.Alias.Arguments.FirstOrDefault(a => a.Name == parameter.Name);
            if (argument != null)
            {
                if (argument.ObjectValue != null)
                {
                    return ResolveObjectValue(argument.ObjectValue, out valueType);
                }
                valueType = argument.ValueType;
                return argument.Value;
            }

            //if argument is not found lookup default value in the Alias Definition
            var paramDef = ((DOM.Antlr.AliasDefinition) aliasContext.AliasDefinition).Parameters.First(p => p.Name == parameter.Name);

            //If parameteres default value is Parameter or Alias then resolve it
            if (paramDef.ObjectValue != null)
            {
                return ResolveObjectValue(paramDef.ObjectValue, out valueType);
            }

            valueType = paramDef.ValueType;
            return paramDef.Value;
        }


        /// <summary>
        /// Go through all entities and resolve attributes for the current node.
        /// </summary>
        /// <param name="entities">List of entities. Looking for alias or parameter because they potentially can hold the attributes.</param>
        protected void ResolveAttributes(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity is Attribute)
                {
                    Visit(entity);
                }
                else if (entity is Alias)
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
            ResolveAttributes(argument != null ? argument.Entities : parameter.Entities);
        }

        private void ResolveAttributesInAlias(Alias alias)
        {
            var aliasDef = _context.NamespaceResolver.GetAliasDefinition(alias.Name);
            AliasContext.Push(new AliasContext() { AliasDefinition = aliasDef, Alias = alias, AliasNsInfo = GetContextNsInfo() });
            ResolveAttributes(aliasDef.Entities);
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

        /// <summary>
        /// Resolves SQS_EOL token. Returns string with new line and indentation symbols.
        /// SQS_EOL token can spread accross several lines.
        /// The first newline symbol is ignored because SQS is "folded" string. 
        /// If there are only one new line symbol in SQS_EOL then it is ignored.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="valueIndent"></param>
        /// <returns></returns>
        private static string ResolveSqsEol(IToken token, int valueIndent)
        {
            var sb = new StringBuilder();
            var value = token.Text;
            var lines = value.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None); //Minimum 2 strings will be in this array

            if(lines.Length < 3) return string.Empty;

            var count = 1;
            foreach (var item in lines)
            {
                //Skip first one
                if (count ++ < 3) //Ignore first empty item and second item with new line symbol.
                {
                    continue;
                }

                //Removing indents
                sb.AppendLine();
                if (item.Length > valueIndent)
                    sb.Append(item.Substring(valueIndent));
            }

            return sb.ToString();
        }

        public override void OnElement(Element node)
        {
            ResolveAttributes(node.Entities);
            Visit(node.Entities.Where(e => !(e is Attribute)));
        }

        public override void OnAlias(Alias alias)
        {
            var aliasDef = _context.NamespaceResolver.GetAliasDefinition(alias.Name);

            AliasContext.Push(new AliasContext() { AliasDefinition = aliasDef, Alias = alias, AliasNsInfo = GetContextNsInfo() });
            Visit(aliasDef.Entities.Where(e => !(e is Attribute)));
            AliasContext.Pop();
        }

        public override void OnParameter(Parameter parameter)
        {
            var aliasContext = AliasContext.Peek();

            if (parameter.Name == "_") //Default parameter. Value is passed in the body of the alias
            {
                Visit(aliasContext.Alias.Entities);
                return;
            }

            var argument = aliasContext.Alias.Arguments.FirstOrDefault(a => a.Name == parameter.Name);

            Visit(argument != null ? argument.Entities : parameter.Entities);
        }

        public override void OnAliasDefinition(AliasDefinition node)
        {
            //Doing nothing for Alias Definition        
        }
    }
}
