using NUnit.Framework;
using static Malina.Parser.Tests.TestUtils;

namespace Malina.Parser.Tests
{
    [TestFixture]
    //[LexerRecord]//Overwrites all recorded tests
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
        public void AliasDefinitionWithInplaceAttributeDefaultParameters()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void AliasDefinitionWithInplaceAttributeParameters()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void AliasDefinitionWithInplaceInlineParameters()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void AliasDefinitionWithSimpleParameters()
        {
            PerformTest();
        }

        [Test,LexerRecorded]
        public void AliasWithArguments()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void AliasWithInlineArgumentList()
        {
            PerformTest();
        }

        [Test,LexerRecorded]
        public void AliasWithInlineArgumentList2()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void AliasWithInlineArgumentList3()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void AliasWithInlineArgumentList4()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void AliasWithInlineArgumentList5()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void AliasWithInlineArguments()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void ElementList1()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void ElementWithAlias()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void ElementWithAliasAndAliasDefinition()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void ElementWithAliasedAttribute()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void ElementWithAttributes()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void ElementWithAttributesAndOtherElements()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void ElementWithNestedAlias()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void ElementWithNestedAliasAndNestedAliasDefinition()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void ElementWithOpenStringValue()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void ElementWithValue()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void EmptyElement()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void EmptyElementWithNamespace()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void InlineElementBody1()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void InlineElementBody2()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void InlineElementBody3()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void InlineElementBody4()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void LineComments()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void ModuleNamespaceOverload()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void ModuleWithTwoNamespaces()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void MultiLineComments()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void NamespaceScope1()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void NamespaceScope2()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void NamespaceScope3()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void OneNamespaceNoBrackets()
        {
            PerformTest();
        }

        [Test, LexerRecorded]
        public void OpenStringEndOnDedentAndEof()
        {
            PerformTest();
        }

        [Test, LexerRecorded]        
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
