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
    public class LexerTests
    {


        [Test]
        public void AliasDefinitionWithAttributes()
        {
            PerformTest();

        }

        [Test]
        public void AliasDefinitionWithAttrAndElem()
        {
            PerformTest();

        }

        [Test]
        public void AliasDefinitionWithDefaultBlockParameters()
        {
            PerformTest();
        }

        [Test]
        public void AliasDefinitionWithDefaultInlineBlockParameters()
        {
            PerformTest();
        }

        [Test]
        public void AliasDefinitionWithInplaceAttributeDefaultParameters()
        {
            PerformTest();
        }

        [Test]
        public void AliasDefinitionWithInplaceAttributeParameters()
        {
            PerformTest();
        }

        [Test]
        public void AliasDefinitionWithInplaceInlineParameters()
        {
            PerformTest();
        }

        [Test]
        public void AliasDefinitionWithSimpleParameters()
        {
            PerformTest();
        }

        [Test]
        public void AliasWithArguments()
        {
            PerformTest();
        }

        [Test]
        public void AliasWithInlineArgumentList()
        {
            PerformTest();
        }

        [Test]
        public void AliasWithInlineArgumentList2()
        {
            PerformTest();
        }

        [Test]
        public void AliasWithInlineArgumentList3()
        {
            PerformTest();
        }

        [Test]
        public void AliasWithInlineArgumentList4()
        {
            PerformTest();
        }

        [Test]
        public void AliasWithInlineArgumentList5()
        {
            PerformTest();
        }

        [Test]
        public void AliasWithInlineArguments()
        {
            PerformTest();
        }

        [Test]
        public void ElementList1()
        {
            PerformTest();
        }

        [Test]
        public void ElementWithAlias()
        {
            PerformTest();
        }

        [Test]
        public void ElementWithAliasAndAliasDefinition()
        {
            PerformTest();
        }

        [Test]
        public void ElementWithAliasedAttribute()
        {
            PerformTest();
        }

        [Test]
        public void ElementWithAttributes()
        {
            PerformTest();
        }

        [Test]
        public void ElementWithAttributesAndOtherElements()
        {
            PerformTest();
        }

        [Test]
        public void ElementWithNestedAlias()
        {
            PerformTest();
        }

        [Test]
        public void ElementWithNestedAliasAndNestedAliasDefinition()
        {
            PerformTest();
        }

        [Test]
        public void ElementWithOpenStringValue()
        {
            PerformTest();
        }

        [Test]
        public void ElementWithValue()
        {
            PerformTest();
        }

        [Test]
        public void EmptyElement()
        {
            PerformTest();
        }

        [Test]
        public void EmptyElementWithNamespace()
        {
            PerformTest();
        }

        [Test]
        public void InlineElementBody1()
        {
            PerformTest();
        }

        [Test]
        public void InlineElementBody2()
        {
            PerformTest();
        }

        [Test]
        public void InlineElementBody3()
        {
            PerformTest();
        }

        [Test]
        public void InlineElementBody4()
        {
            PerformTest();
        }

        [Test]
        public void LineComments()
        {
            PerformTest();
        }

        [Test]
        public void ModuleNamespaceOverload()
        {
            PerformTest();
        }

        [Test]
        public void ModuleWithTwoNamespaces()
        {
            PerformTest();
        }

        [Test]
        public void MultiLineComments()
        {
            PerformTest();
        }

        [Test]
        public void NamespaceScope1()
        {
            PerformTest();
        }

        [Test]
        public void NamespaceScope2()
        {
            PerformTest();
        }

        [Test]
        public void NamespaceScope3()
        {
            PerformTest();
        }

        [Test]
        public void OneNamespaceNoBrackets()
        {
            PerformTest();
        }

        [Test]
        public void OpenStringEndOnDedentAndEof()
        {
            PerformTest();
        }

        [Test]
        public void OpenStringMiltiline()
        {
            PerformTest();
        }

        [Test]
        public void OpenStringSimple()
        {
            PerformTest();
        }

        [Test]
        public void TwoNamespaces()
        {
            PerformTest();
        }

        [Test]
        public void WithoutNamespaces()
        {
            PerformTest();
        }


    }
}
