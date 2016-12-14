using Malina.DOM;
using Malina.Parser.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using static Malina.Compiler.Tests.TestUtils;

namespace Malina.Compiler.Tests
{
    [TestFixture]
    public class CompilerTestFixture
    {
        [Test, RecordedTest]
        public void ModulesWithNsDocumentAndNsAlias()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void NestedAliases()
        {
            PerformCompilerTest();
        }

        [Test]
        public void MissingAlias()
        {
            var errorsExpected = new List<CompilerError>()
            {
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 7,3,-1), "Alias 'Address.UK.Cambridge' is not defined.")
            };
            PerformCompilerTest(errorsExpected);
        }

        [Test]
        public void NamespaceIsNotDefined()
        {
            var errorsExpected = new List<CompilerError>()
            {
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 3,1,-1), "Namespace prefix 'ipo' is not defined."),
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 4,2,-1), "Namespace prefix 'ipo2' is not defined.")
            };
            PerformCompilerTest(errorsExpected);
        }


        [Test]
        public void DuplicateDocumentName()
        {
            var errorsExpected = new List<CompilerError>()
            {
                new CompilerError(new LexicalInfo("ModuleWithDocument1.mlx", 2,1,-1), "Duplicate document name - 'PurchaseOrder'."),
                new CompilerError(new LexicalInfo("ModuleWithDocument2.mlx", 2,1,-1), "Duplicate document name - 'PurchaseOrder'.")
            };
            PerformCompilerTest(errorsExpected);
        }

        [Test]
        public void DuplicateAliasDefinition()
        {
            var errorsExpected = new List<CompilerError>()
            {
                new CompilerError(new LexicalInfo("ModuleWithAliasDef1.mlx", 1,1,-1), "Duplicate alias definition name - 'Address'."),
                new CompilerError(new LexicalInfo("ModuleWithAliasDef2.mlx", 2,1,-1), "Duplicate alias definition name - 'Address'.")
            };
            PerformCompilerTest(errorsExpected);
        }

        [Test]
        public void AliasHasCircularReference()
        {
            var errorsExpected = new List<CompilerError>()
            {
                new CompilerError(new LexicalInfo("ModuleWithAlias1.mlx", 2,1,-1), "Alias Definition 'Address1' has circular reference."),
                new CompilerError(new LexicalInfo("ModuleWithAlias1.mlx", 9,1,-1), "Alias Definition 'Address3' has circular reference."),
                new CompilerError(new LexicalInfo("ModuleWithAlias1.mlx", 16,1,-1), "Alias Definition 'Address4' has circular reference."),
                new CompilerError(new LexicalInfo("ModuleWithAlias2.mlx", 2,1,-1), "Alias Definition 'Address2' has circular reference."),
                new CompilerError(new LexicalInfo("ModuleWithAlias2.mlx", 9,1,-1), "Alias Definition 'Address5' has circular reference."),
                new CompilerError(new LexicalInfo("ModuleWithAlias2.mlx", 17,1,-1), "Alias Definition 'Address6' has circular reference.")
            };
            PerformCompilerTest(errorsExpected);
        }


        
        [Test, RecordedTest]
        public void AliasInsideSqs()
        {
            PerformCompilerTest();
        }


        [Test, RecordedTest]
        public void AliasWithArguments()
        {
            PerformCompilerTest();
        }


        [Test, RecordedTest]
        public void AliasWithDefaultBlockParameters()
        {
            PerformCompilerTest();
        }

        [Test]
        public void ExtraRootElementInDocument()
        {
            var errorsExpected = new List<CompilerError>()
            {
                new CompilerError(new LexicalInfo("ModuleWithDocument1.mlx", 2, 1,-1), "Document 'PurchaseOrder' must have only one root element."),
                new CompilerError(new LexicalInfo("ModuleWithDocument1.mlx", 12, 1,-1), "Document 'PurchaseOrder2' must have only one root element."),
                new CompilerError(new LexicalInfo("ModuleWithDocument1.mlx", 22, 1,-1), "Alias Definition 'Address1' has circular reference."),
                new CompilerError(new LexicalInfo("ModuleWithDocument1.mlx", 26, 1,-1), "Alias Definition 'Address2' has circular reference."),
                new CompilerError(new LexicalInfo("ModuleWithDocument1.mlx", 30, 1,-1), "Document 'PurchaseOrder3' must have only one root element."),
                new CompilerError(new LexicalInfo("ModuleWithDocument1.mlx", 34, 1,-1), "Document 'PurchaseOrder4' must have at least one root element."),
            };
            PerformCompilerTest(errorsExpected);
        }

        [Test, RecordedTest]
        public void JsonArray()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void JsonArrayInAlias()
        {
            PerformCompilerTest();
        }

        [Test]
        public void JsonArrayItemInObject()
        {
            var errorsExpected = new List<CompilerError>()
            {
                new CompilerError(new LexicalInfo("JsonArray.mlj", 5, 3,-1), "Object property is expected."),
                new CompilerError(new LexicalInfo("JsonArray.mlj", 8, 3,-1), "Object property is expected."),
            };
            PerformCompilerTest(errorsExpected);
        }

        [Test]
        public void JsonPropertyInArray()
        {
            var errorsExpected = new List<CompilerError>()
            {
                new CompilerError(new LexicalInfo("JsonArray.mlj", 6, 3,-1), "Array item is expected."),
                new CompilerError(new LexicalInfo("JsonArray.mlj", 7, 3,-1), "Array item is expected."),
                new CompilerError(new LexicalInfo("JsonArray.mlj", 11, 3,-1), "Array item is expected."),
                new CompilerError(new LexicalInfo("JsonArray.mlj", 15, 2,-1), "Array item is expected."),

            };
            PerformCompilerTest(errorsExpected);
        }

        [Test, RecordedTest]
        public void AliasWithDefaultValueParameters()
        {
            PerformCompilerTest();
        }

        [Test]
        public void ParameterInDocument()
        {
            var errorsExpected = new List<CompilerError>()
            {
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 7, 4,-1), "Parameters can't be declared in documents."),
            };
            PerformCompilerTest(errorsExpected);
        }

        [Test]
        public void SchemaValidation()
        {
            var errorsExpected = new List<CompilerError>()
            {
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 4, 1,-1), "XML validation error - 'The element 'purchaseOrder' in namespace 'http://www.example.com/myipo' has incomplete content. List of possible elements expected: 'comment' in namespace 'http://www.example.com/myipo' as well as 'Items'.'."),
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 5, 2,-1), "XML validation error - 'The element 'shipTo' has incomplete content. List of possible elements expected: 'name, street' in namespace 'http://www.example.com/myipo'.'."),
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 6, 2,-1), "XML validation error - 'The element 'billTo' has incomplete content. List of possible elements expected: 'name, street' in namespace 'http://www.example.com/myipo'.'."),
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 12, 1,-1), "XML validation error - 'The element 'purchaseOrder' in namespace 'http://www.example.com/myipo' has incomplete content. List of possible elements expected: 'shipTo'.'."),
            };
            PerformCompilerTest(errorsExpected);
        }


        [Test]
        public void SchemaValidationXsdMissing()
        {
            var errorsExpected = new List<CompilerError>()
            {
                new CompilerError(new LexicalInfo("ipo.xsd", 15, 5,-1), "XML validation error - 'Type 'http://www.example.com/myipo:Address' is not declared.'."),
            };
            PerformCompilerTest(errorsExpected);
        }

        [Test]
        public void AliasWithIncorrectBlock()
        {
            var errorsExpected = new List<CompilerError>()
            {
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 9, 5,-1), "ParserError - 'Unexpected token ELEMENT_ID<'City'>'."),
            };
            PerformCompilerTest(errorsExpected);
        }

        [Test]
        public void AliasWithIncorrectType()
        {
            var errorsExpected = new List<CompilerError>()
            {
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 4, 4,-1), "Can not use value alias in the block."),
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 12, 11,-1), "Can not use block alias as value."),
            };
            PerformCompilerTest(errorsExpected);
        }



        [Test]
        public void AliasWithMissedArgument()
        {
            var errorsExpected = new List<CompilerError>()
            {
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 4, 4,-1), "Argument 'street' is missing."),
            };
            PerformCompilerTest(errorsExpected);
        }

        [Test, RecordedTest]
        public void AliasWithAttributes()
        {
            PerformCompilerTest();
        }

        [Test, RecordedTest]
        public void ArgumentWithObjectValue()
        {
            PerformCompilerTest();
        }

        [Test]
        public void AliasWithDuplicateArguments()
        {
            var errorsExpected = new List<CompilerError>()
            {
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 6, 5,-1), "Duplicate argument name - 'name'."),
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 7, 5,-1), "Duplicate argument name - 'name'."),
            };
            PerformCompilerTest(errorsExpected);
        }

        [Test]
        public void AliasWithIncorrectArgumentType()
        {
            var errorsExpected = new List<CompilerError>()
            {
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 5, 5,-1), "Value argument is expected."),
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 7, 5,-1), "Block argument is expected."),
            };
            PerformCompilerTest(errorsExpected);
        }

        [Test, RecordedTest]
        public void TwoModulesWithDocumentAndAlias()
        {
            PerformCompilerTest();
        }

        [Test]
        public void UndeclaredNamespace()
        {
            var errorsExpected = new List<CompilerError>()
            {
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 2, 1,-1), "Namespace prefix 'ipo' is not defined."),
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 5, 1,-1), "Namespace prefix 'ipo2' is not defined."),
            };
            PerformCompilerTest(errorsExpected);
        }

        [Test]
        public void UnresolvedAliasInsideSqs()
        {
            var errorsExpected = new List<CompilerError>()
            {
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 4, 7,-1), "Alias 'bla' is not defined."),
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 5, 15,-1), "Alias 'user.first' is not defined."),
                new CompilerError(new LexicalInfo("ModuleWithDocument.mlx", 5, 30,-1), "Alias 'user.last' is not defined."),
            };
            PerformCompilerTest(errorsExpected);
        }

    }
}
