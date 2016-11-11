using System;
using NUnit.Framework;
using static Malina.Parser.Tests.TestUtils;
using Antlr4.Runtime;
using Malina.Parser.Tests;
using Antlr4.Runtime.Atn;
using Malina.DOM;

namespace Malina.Parser.PerfomanceTests
{
    [TestFixture]
    public class PerformanceTests
    {
        [Test]
        public void BigFile()
        {
            Console.WriteLine("Starting BigFile");
            var code = LoadTestCodeRaw();
            var lexer = new MalinaLexer(new AntlrInputStream(code));
            //lexer.RemoveErrorListeners();
            //lexer.ErrorListeners.Clear();
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
            Assert.Less(t2 - t1, 7000);

            lexer.Reset();
            var parser = new MalinaParser(new CommonTokenStream(lexer));
            parser.Interpreter.PredictionMode = PredictionMode.Sll;
            var malinaListener = new MalinaParserListener();
            var parserErrorListener = new ErrorListener<IToken>();
            parser.AddErrorListener(parserErrorListener);
            //parser.AddParseListener(malinaListener);
            parser.BuildParseTree = false;
            t1 = Environment.TickCount;
            //var module = parser.module();
            t2 = Environment.TickCount;

            Console.WriteLine("Parse Time: {0}", t2 - t1);
            Assert.Less(t2 - t1,15000);
            Assert.IsFalse(parserErrorListener.HasErrors);


            lexer.Reset();
            parser.Reset();
            parser.AddParseListener(malinaListener);            
            t1 = Environment.TickCount;            
            //module = parser.module();
            t2 = Environment.TickCount;
            Console.WriteLine("DOM Time: {0}", t2 - t1);
            Assert.Less(t2 - t1 , 25000);
            Assert.IsFalse(parserErrorListener.HasErrors);

            t1 = Environment.TickCount;
            var visitor = new DOMPrinterVisitor();
            //visitor.Visit(malinaListener.CompileUnit);
            t2 = Environment.TickCount;
            Console.WriteLine("Visitor Time: {0}", t2 - t1);


            //GetAllTokens Time: 5625
            //Token Number: 7764705
            //Parse Time: 0
            //DOM Time: 23516
            //Visitor Time: 6828

            //GetAllTokens Time: 5734
            //Token Number: 7764705
            //Parse Time: 14391
            //DOM Time: 12375
            //Visitor Time: 6046
        }

        [Test]
        public void BigFile2()
        {
            Console.WriteLine("Starting BigFile");
            var code = LoadTestCodeRaw();
            var lexer = new MalinaLexer(new AntlrInputStream(code));
            //lexer.RemoveErrorListeners();
            //lexer.ErrorListeners.Clear();
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
            Assert.Less(t2 - t1, 7000);

            lexer.Reset();
            var parser = new MalinaParser(new CommonTokenStream(lexer));
            parser.Interpreter.PredictionMode = PredictionMode.Sll;
            var malinaListener = new MalinaParserListener();
            var parserErrorListener = new ErrorListener<IToken>();
            parser.AddErrorListener(parserErrorListener);
            //parser.AddParseListener(malinaListener);
            parser.BuildParseTree = false;
            t1 = Environment.TickCount;
            //var module = parser.module();
            t2 = Environment.TickCount;

            Console.WriteLine("Parse Time: {0}", t2 - t1);
            Assert.Less(t2 - t1, 15000);
            Assert.IsFalse(parserErrorListener.HasErrors);


            lexer.Reset();
            parser.Reset();
            parser.AddParseListener(malinaListener);
            t1 = Environment.TickCount;
            var module = parser.module();
            lexer.Reset();
            parser.Reset();
            parser = null;
            t2 = Environment.TickCount;
            Console.WriteLine("DOM Time: {0}", t2 - t1);
            Assert.Less(t2 - t1, 20000);
            Assert.IsFalse(parserErrorListener.HasErrors);

            t1 = Environment.TickCount;
            var visitor = new DOMPrinterVisitor();
            visitor.Visit(malinaListener.CompileUnit);            
            t2 = Environment.TickCount;
            Console.WriteLine("Visitor Time: {0}", t2 - t1);
        }

    }
}
