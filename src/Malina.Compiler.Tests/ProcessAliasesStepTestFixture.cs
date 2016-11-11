using NUnit.Framework;
using Malina.Compiler.Tests;
using Malina.Parser.Tests;

namespace Malina.Compiler.Tests
{
    [TestFixture]
    class ProcessAliasesStepTestFixture
    {
        [Test, DomRecorded]
        public void ModuleWithDocumentAndAlias()
        {
            var cc = TestUtils.PerformProcessAliasesTest();


        }
    }
}
