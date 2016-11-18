using Malina.DOM;
using NUnit.Framework;
using System.Collections.Generic;
using static Malina.Compiler.Tests.TestUtils;

namespace Malina.Compiler.Tests
{
    [TestFixture]
    public class CompilerTestFixture
    {
        [Test, RecordedTest]
        public void TwoModulesWithDocumentAndAlias()
        {
            PerformCompilerTest();
        }

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
      }
}
