using Antlr4.Runtime;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Malina.Parser.Tests.TestUtils;

namespace Malina.Parser.Tests
{
    [TestFixture]
    public class LexerParserTests
    {


        [Test, RecordTest]
        public void AliasDefinitionWithAttributes()
        {
            PerformTest();

        }

        [Test, RecordedTest]
        public void AliasDefinitionWithAttrAndElem()
        {
            PerformTest();

        }

        [Test, RecordedTest]
        public void AliasDefinitionWithDefaultBlockParameters()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void AliasDefinitionWithDefaultInlineBlockParameters()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void AliasDefinitionWithInplaceAttributeDefaultParameters()
        {
            PerformTest();
        }

        [Test,RecordedTest]
        public void AliasDefinitionWithInplaceAttributeParameters()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void AliasDefinitionWithInplaceInlineParameters()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void AliasDefinitionWithSimpleParameters()
        {
            PerformTest();
        }

        [Test,RecordedTest]
        public void AliasWithArguments()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void AliasWithInlineArgumentList()
        {
            PerformTest();
        }

        [Test,RecordedTest]
        public void AliasWithInlineArgumentList2()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void AliasWithInlineArgumentList3()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void AliasWithInlineArgumentList4()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void AliasWithInlineArgumentList5()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void AliasWithInlineArguments()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void ElementList1()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void ElementWithAlias()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void ElementWithAliasAndAliasDefinition()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void ElementWithAliasedAttribute()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void ElementWithAttributes()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void ElementWithAttributesAndOtherElements()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void ElementWithNestedAlias()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void ElementWithNestedAliasAndNestedAliasDefinition()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void ElementWithOpenStringValue()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void ElementWithValue()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void EmptyElement()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void EmptyElementWithNamespace()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void InlineElementBody1()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void InlineElementBody2()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void InlineElementBody3()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void InlineElementBody4()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void LineComments()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void ModuleNamespaceOverload()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void ModuleWithTwoNamespaces()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void MultiLineComments()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void NamespaceScope1()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void NamespaceScope2()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void NamespaceScope3()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void OneNamespaceNoBrackets()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void OpenStringEndOnDedentAndEof()
        {
            PerformTest();
        }

        [Test, RecordedTest]        
        public void OpenStringMiltiline()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void OpenStringSimple()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void TwoNamespaces()
        {
            PerformTest();
        }

        [Test, RecordedTest]
        public void WithoutNamespaces()
        {
            PerformTest();
        }


    }
}
