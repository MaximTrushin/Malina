﻿#region license
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
using Malina.DOM;
using System;
using System.Xml.Schema;

namespace Malina.Compiler
{
    public static class CompilerErrorFactory
    {
        public static CompilerError InputError(string inputName, Exception x)
        {
            return InputError(new LexicalInfo(inputName), x);
        }

        public static CompilerError InputError(LexicalInfo lexicalInfo, Exception error)
        {
            return Instantiate("MCE0001", lexicalInfo, error, lexicalInfo.FileName, error.Message);
        }

        internal static CompilerError FatalError(Exception ex)
        {
            return new CompilerError("MCE0000", ex, ex.Message);
        }

        internal static CompilerError FatalError(Exception ex, string message)
        {
            return new CompilerError("MCE0000", ex, message);
        }

        private static CompilerError Instantiate(string code, LexicalInfo location, Exception error, params object[] args)
        {
            return new CompilerError(code, location, error, args);
        }

        private static CompilerError Instantiate(string code, LexicalInfo location, params object[] args)
        {
            return new CompilerError(code, location, Array.ConvertAll(args, DisplayStringFor));
        }

        internal static string DisplayStringFor(object o)
        {
            if (o == null) return "";

            return  o.ToString();
        }

        internal static Exception FileNotFound(string fileName)
        {
            return Instantiate("MCE0002", new LexicalInfo(fileName), fileName);
        }

        internal static CompilerError NsPrefixNotDefined<T>(T node, string fileName) where T : Node
        {
            return Instantiate("MCE0003", new LexicalInfo(fileName, node.start.Line, node.start.Column, node.start.Index), ((INsNode) node).NsPrefix);
        }

        internal static CompilerError AliasIsNotDefined(Alias alias, string fileName)
        {
            return Instantiate("MCE0004", new LexicalInfo(fileName, alias.start.Line, alias.start.Column, alias.start.Index), alias.Name);
        }

        internal static CompilerError AliasDefHasCircularReference(NsInfo aliasDefNsInfo)
        {
            var aliasDef = (AliasDefinition) aliasDefNsInfo.ModuleMember;
            return Instantiate("MCE0005", new LexicalInfo(aliasDef.Module.FileName,
                aliasDef.start.Line, aliasDef.start.Column + 1, aliasDef.start.Index), aliasDef.Name);
        }

        internal static CompilerError LexerError(Exception e, string message, string fileName, int line, int column)
        {
            return Instantiate("MCE0006", new LexicalInfo(fileName, line, column, 0), message);
        }

        internal static CompilerError ParserError(Exception e, string message, string fileName, int line, int column)
        {
            return Instantiate("MCE0007", new LexicalInfo(fileName, line, column, 0), message);
        }

        internal static CompilerError DuplicateDocumentName(DOM.Document document, string fileName)
        {
            return Instantiate("MCE0008", new LexicalInfo(fileName, document.start.Line, document.start.Column + 1, document.start.Index), document.Name);
        }

        internal static CompilerError DocumentMustHaveOneRootElement(DOM.Document document, string fileName, string only)
        {
            return Instantiate("MCE0009", new LexicalInfo(fileName, document.start.Line, document.start.Column + 1, document.start.Index), document.Name, only);
        }
        internal static CompilerError ParametersCantBeDeclaredInDocuments(DOM.Parameter node, string fileName)
        {
            return Instantiate("MCE0010", new LexicalInfo(fileName, node.start.Line, node.start.Column + 1, node.start.Index));
        }

        internal static CompilerError DuplicateArgumentName(Argument argument, string fileName)
        {
            return Instantiate("MCE0011", new LexicalInfo(fileName, argument.start.Line, argument.start.Column + 1, argument.start.Index), argument.Name);
        }

        internal static CompilerError DuplicateAliasDefName(AliasDefinition aliasDef, string fileName)
        {
            return Instantiate("MCE0012", new LexicalInfo(fileName, aliasDef.start.Line, aliasDef.start.Column + 1, aliasDef.start.Index), aliasDef.Name);
        }

        internal static CompilerError ArgumentIsMissing(Alias alias, string argumentName, string fileName)
        {
            return Instantiate("MCE0013", new LexicalInfo(fileName, alias.start.Line, alias.start.Column + 1, alias.start.Index), argumentName);
        }

        internal static CompilerError ValueArgumentIsExpected(Argument argument, string fileName)
        {
            return Instantiate("MCE0014", new LexicalInfo(fileName, argument.start.Line, argument.start.Column + 1, argument.start.Index));
        }

        internal static CompilerError BlockArgumentIsExpected(Argument argument, string fileName)
        {
            return Instantiate("MCE0015", new LexicalInfo(fileName, argument.start.Line, argument.start.Column + 1, argument.start.Index));
        }

        internal static CompilerError CantUseValueAliasInTheBlock(Alias alias, string fileName)
        {
            return Instantiate("MCE0016", new LexicalInfo(fileName, alias.start.Line, alias.start.Column + 1, alias.start.Index));
        }

        internal static CompilerError CantUseBlockAliasAsValue(Alias alias, string fileName)
        {
            return Instantiate("MCE0017", new LexicalInfo(fileName, alias.start.Line, alias.start.Column + 1, alias.start.Index));
        }

        internal static CompilerError XmlSchemaValidationError(XmlSchemaValidationException ex)
        {
            return Instantiate("MCE0018", new LexicalInfo(ex.SourceUri, ex.LineNumber, ex.LinePosition, 0), ex.Message);
        }

        internal static CompilerError XmlSchemaValidationError(XmlSchemaException ex, LexicalInfo location)
        {
            return Instantiate("MCE0018", new LexicalInfo(location.FileName, location.Line, location.Column, 0), ex.Message);
        }

        internal static CompilerError ArrayItemIsExpected(Node node, string fileName)
        {
            return Instantiate("MCE0019", new LexicalInfo(fileName, node.start.Line, node.start.Column + 1, node.start.Index));
        }

        public static CompilerError PropertyIsExpected(Node node, string fileName)
        {
            return Instantiate("MCE0020", new LexicalInfo(fileName, node.start.Line, node.start.Column + 1, node.start.Index));
        }

        public static CompilerError DefaultParameterMustBeOnly(Node node, string fileName)
        {
            return Instantiate("MCE0021", new LexicalInfo(fileName, node.start.Line, node.start.Column + 1, node.start.Index));
        }

        public static CompilerError ArgumentMustBeDefinedInAlias(Argument node, string fileName)
        {
            return Instantiate("MCE0022", new LexicalInfo(fileName, node.start.Line, node.start.Column + 1, node.start.Index));
        }

        public static CompilerError DefaultBlockArgumentIsMissing(Alias alias, string fileName)
        {
            return Instantiate("MCE0023", new LexicalInfo(fileName, alias.start.Line, alias.start.Column + 1, alias.start.Index));
        }

        public static CompilerError UnexpectedArgument(Argument argument, string fileName)
        {
            return Instantiate("MCE0024", new LexicalInfo(fileName, argument.start.Line, argument.start.Column + 1, argument.start.Index));
        }

        public static CompilerError UnexpectedDefaultBlockArgument(Entity entity, string fileName)
        {
            return Instantiate("MCE0025", new LexicalInfo(fileName, entity.start.Line, entity.start.Column + 1, entity.start.Index));
        }

        public static CompilerError DefaultValueArgumentIsMissing(Alias alias, string fileName)
        {
            return Instantiate("MCE0026", new LexicalInfo(fileName, alias.start.Line, alias.start.Column + 1, alias.start.Index));
        }

        public static CompilerError UnexpectedDefaultValueArgument(Alias alias, string fileName)
        {
            return Instantiate("MCE0027", new LexicalInfo(fileName, alias.start.Line, alias.start.Column + 1, alias.start.Index));
        }

        public static CompilerError CantDefineArrayItemInXmlDocument(Element node, string fileName)
        {
            return Instantiate("MCE0028", new LexicalInfo(fileName, node.start.Line, node.start.Column + 1, node.start.Index));
        }
    }
}