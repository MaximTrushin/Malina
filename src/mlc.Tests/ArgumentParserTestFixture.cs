using System.Collections.Generic;
using NUnit.Framework;


namespace mlc.Tests
{
    [TestFixture, Category("ArgumentParser")]
    public class ArgumentParserTestFixture
    {
        [Test]
        public void NoFilesSpecified()
        {
            List<string> files;
            bool recursive;
            string outputDirectory;
            Assert.Throws<ArgumentsParserException>(
                () => ArgumentsParser.Parse(new string[] {}, out files, out recursive, out outputDirectory),
                ArgumentsParser.ParameterErrors.NoInput);
        }
    }
}