using NUnit.Framework;
using Malina.Compiler;


namespace mlc.Tests
{
    [TestFixture]
    public class ArgumentParserTestFixture
    {
        [Test]
        public void NoFilesSpecified()
        {
            var cp = new CompilerParameters();
            Assert.Throws<ArgumentsParserException>(() => ArgumentsParser.Parse(new string[] { }, cp), ArgumentsParser.ParameterErrors.NoInput);            
        }
    }
}
