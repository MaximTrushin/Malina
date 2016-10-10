using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using static Malina.Parser.Tests.TestUtils;
using Antlr4.Runtime;

namespace Malina.Parser.PerfomanceTests
{
    [TestFixture]
    public class PerformanceTests
    {
        [Test]
        public void BigFile()
        {
            Console.WriteLine("Starting BigFile");
            var code = LoadTestCode();
            var lexer = new MalinaLexer(new AntlrInputStream(code));
            lexer.RemoveErrorListeners();
            lexer.ErrorListeners.Clear();
            var t1 = Environment.TickCount;
            var i = 0;
            IToken token = null;
            for (token = lexer.NextToken(); token.Type != -1; token = lexer.NextToken())
            {
                i++;
            }
            var t2 = Environment.TickCount;

            Console.WriteLine("GetAllTokens Time: {0}", t2 - t1);
            Console.WriteLine("Token Number: {0}", i);
            Assert.IsTrue(t2 - t1 < 10000);
        }
    }
}
