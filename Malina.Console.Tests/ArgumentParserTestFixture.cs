using NUnit.Framework;
using Malina.Console;
using Malina.Compiler;
using System;

namespace Malina.Console.Tests
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
