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
                new CompilerError(new LexicalInfo(null, 7,3,78), "Alias 'Address.UK.Cambridge' is not defined.")
            };
            PerformCompilerTest(errorsExpected);
        }

    }
}
