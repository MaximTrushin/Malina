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
using NUnit.Framework;
using static Malina.Parser.Tests.TestUtils;

namespace Malina.Parser.Tests
{
    /// <summary>
    /// Each test can perform 5 type of test:
    /// 1) Lexer - compares generated stream of Tokens with the recorded one
    /// 2) LexerError - compares lexer errors with recorded lexer errors
    /// 3) Parser - compares ParseTree with recorded parse tree
    /// 4) ParserError - compare parser errors with recorded parser errors
    /// 5) Dom - compare generated Dom structure with the recorded Dom structure
    /// </summary>
    [TestFixture, Category("LexerParser")]
    public class LexerParserTests
    {


        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void AliasDefinitionWithAttributes()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void AliasDefinitionWithAttrAndElem()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void AliasDefinitionWithDefaultBlockParameters()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void AliasDefinitionWithDefaultInlineBlockParameters()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void AliasDefinitionWithDefaultValueParameters()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void AliasDefinitionWithInlineAttributeDefaultParameters()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void AliasDefinitionWithInlineAttributeParameters()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void AliasDefinitionWithInlineParameters()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void AliasDefinitionWithSimpleParameters()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void AliasWithArguments()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void AliasWithDefaultValueParameter()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void AliasWithIncorrectBlock()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void AliasWithInlineArgumentList()
        {
            PerformTest();
        }

        [Test,LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void AliasWithInlineArgumentList2()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void AliasWithInlineArgumentList3()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void AliasWithInlineArgumentList4()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void AliasWithInlineArgumentList5()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void AliasWithInlineArguments()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void DotEscapedInId()
        {
            PerformTest();
        }

        [Test, LexerRecord, LexerErrorRecorded, ParseTreeRecorded, DomRecorded]
        public void DoubleQuoteMultilineString()
        {
            PerformTest();
        }

        [Test, LexerRecorded, LexerErrorRecorded, ParseTreeRecorded, DomRecorded]
        public void DoubleQuoteMultilineStringEof()
        {
            PerformTest();
        }

        [Test, LexerRecorded, LexerErrorRecorded, ParseTreeRecorded, DomRecorded]
        public void DoubleQuoteMultilineStringEof2()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void ElementList1()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void ElementWithAlias()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void ElementWithAliasAndAliasDefinition()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void ElementWithAliasedAttribute()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void ElementWithAttributes()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void ElementWithAttributesAndOtherElements()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void ElementWithNestedAlias()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void ElementWithNestedAliasAndNestedAliasDefinition()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void ElementWithOpenStringValue()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void ElementWithValue()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void ElementWithNamespace()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void EmptyElement()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void EmptyElementWithNamespace()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void EmptyParameters()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void FreeOpenString()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void HybridBlock()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void HybridBlockAliasStmt()
        {
            PerformTest();
        }
       
        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void HybridBlockArgumentStmt()
        {
            PerformTest();
        }
        
        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void HybridBlockParameterStmt()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void InlineAliasDefinition()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void InlineAliasEndOfBlock()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void InlineDocumentDefinition()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void InlineElementBody1()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void InlineElementBody2()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void InlineElementBody3()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void InlineElementBody4()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void InlineJsonArray()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void JsonArray()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void JsonArrayWithValues()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void JsonArrayWithValuesInParameters()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void JsonArrayItemInObject()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void JsonEmptyArrayAndObject()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void JsonLiteralsInSqs()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void LineComments()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void LineCommentsInQuotedStrings()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void ModuleNamespaceOverload()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void ModuleWithTwoNamespaces()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void NamespaceScope1()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void NamespaceScope2()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void NamespaceScope3()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void OneNamespaceNoBrackets()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void OpenStringEndOnDedentAndEof()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void OpenStringEndsWithIndentEof()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]        
        public void OpenStringMultiline()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void OpenStringMultiline2()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void OpenStringMultiline3()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void OpenStringSimple()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void SingleQuoteInterpolation()
        {
            PerformTest();
        }

        [Test, LexerRecorded, LexerErrorRecorded, ParseTreeRecorded, DomRecorded]
        public void SingleQuoteMultiline()
        {
            PerformTest();
        }

        [Test, LexerRecorded, LexerErrorRecorded, ParseTreeRecorded, DomRecorded]
        public void SingleQuotedStringInline()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void SingleQuoteEscape()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void OpenStringEmpty()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void TwoNamespaces()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void ValueAliasDefinition()
        {
            PerformTest();
        }


        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void ElementAtEOF()
        {
            PerformTest();
        }

        [Test, LexerRecorded, ParseTreeRecorded, DomRecorded]
        public void Wsa1()
        {
            PerformTest();
        }

        [Test, LexerRecorded, LexerErrorRecorded, ParseTreeRecorded, DomRecorded]
        public void Wsa2()
        {
            PerformTest();
        }
    }
}
