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

    }
}
