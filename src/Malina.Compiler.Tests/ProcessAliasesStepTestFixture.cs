using NUnit.Framework;
using Malina.Compiler.Tests;

namespace Malina.Compiler.Tests
{
    [TestFixture]
    class ProcessAliasesStepTestFixture
    {
        [Test]
        public void ModuleWithDocumentAndAlias()
        {
            TestUtils.PerformProcessAliasesTest();
        }
    }
}
