using NUnit.Framework;
using static Malina.Compiler.Tests.TestUtils;

namespace Malina.Compiler.Tests
{
    [TestFixture]
    public class CompilerTestFixture
    {
        [Test, RecordedTest]
        public void ModuleWithDocumentAndAlias()
        {
            PerformCompilerTest();
        }
    }
}
