using NUnit.Framework;
using static Malina.Parser.Tests.TestUtils;

namespace Malina.Parser.Tests
{
    [TestFixture]
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
        public void LineComments()
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
        public void MultiLineComments()
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
        public void OpenStringMiltiline()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void OpenStringSimple()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void TwoNamespaces()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void WithoutNamespaces()
        {
            PerformTest();
        }


    }
}
