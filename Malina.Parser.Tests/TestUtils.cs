using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Malina.Parser.Tests
{
    public class TestUtils
    {
        public static string AssemblyDirectory
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }

        public static string LoadTestCode()
        {
            var testCaseName = GetTestCaseName();

            var fileName = new StringBuilder(AssemblyDirectory + @"\Scenarios\Lexer\").Append(testCaseName).Append(".mal").ToString();

            return File.ReadAllText(fileName).Replace("\r\n", "\n");
        }

        public static string LoadTestCodeRaw()
        {
            var testCaseName = GetTestCaseName();

            var fileName = new StringBuilder(AssemblyDirectory + @"\Scenarios\Lexer\").Append(testCaseName).Append(".mal").ToString();

            return File.ReadAllText(fileName);
        }

        private static string GetTestCaseName()
        {
            var trace = new StackTrace();
            return trace.GetFrames().Select(f => f.GetMethod()).Where(m => m.CustomAttributes.Any(a => a.AttributeType.FullName == "NUnit.Framework.TestAttribute")).First().Name;
        }

        public static string LoadRecordedTest()
        {
            var testCaseName = GetTestCaseName();
            var fileName = new StringBuilder(AssemblyDirectory + @"\Scenarios\Lexer\Recorded\").Append(testCaseName).Append(".test").ToString();
            if (!File.Exists(fileName)) return null;

            return File.ReadAllText(fileName).Replace("\r\n", "\n");
        }

        private static void SaveRecordedTest(string printedTokens)
        {
            var testCaseName = GetTestCaseName();
            var fileName = new StringBuilder(AssemblyDirectory + @"\Scenarios\Lexer\Recorded\").Append(testCaseName).Append(".test").ToString();
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            File.WriteAllText(fileName, printedTokens);
        }

        public static string LoadRecordedParseTreeTest()
        {
            var testCaseName = GetTestCaseName();
            var fileName = new StringBuilder(AssemblyDirectory + @"\Scenarios\Lexer\Recorded\").Append(testCaseName).Append(".tree").ToString();
            if (!File.Exists(fileName)) return null;

            return File.ReadAllText(fileName).Replace("\r\n", "\n");
        }
        
        private static void SaveRecordedParseTreeTest(string parseTree)
        {
            var testCaseName = GetTestCaseName();
            var fileName = new StringBuilder(AssemblyDirectory + @"\Scenarios\Lexer\Recorded\").Append(testCaseName).Append(".tree").ToString();
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            File.WriteAllText(fileName, parseTree);
        }

        public static string LoadRecordedDomTest()
        {
            var testCaseName = GetTestCaseName();
            var fileName = new StringBuilder(AssemblyDirectory + @"\Scenarios\Lexer\Recorded\").Append(testCaseName).Append(".dom").ToString();
            if (!File.Exists(fileName)) return null;

            return File.ReadAllText(fileName).Replace("\r\n", "\n");
        }

        private static void SaveRecordedDomTest(string printedTokens)
        {
            var testCaseName = GetTestCaseName();
            var fileName = new StringBuilder(AssemblyDirectory + @"\Scenarios\Lexer\Recorded\").Append(testCaseName).Append(".dom").ToString();
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            File.WriteAllText(fileName, printedTokens);
        }

        public static void PerformTest()
        {
            var code = LoadTestCode();

            PrintCode(code);

            //Testing Lexer
            MalinaLexer lexer;
            ErrorListener<int> lexerErros;
            IList<IToken> tokens =  GetTokens(code, out lexer, out lexerErros);

            var printedTokens = PrintedTokens(tokens);
            Console.WriteLine("");
            Console.WriteLine("Tokens:");
            Console.WriteLine(printedTokens);

            var isLexerRecordedTest = IsLexerRecordedTest();
            var isLexerRecordTest = IsLexerRecordTest(); //Overwrites existing recording
            string recorded = null;
            if (isLexerRecordedTest || isLexerRecordTest)
            {
                if (isLexerRecordedTest) recorded = LoadRecordedTest();
                if (recorded == null || isLexerRecordTest)
                {
                    SaveRecordedTest(printedTokens);
                }
            }

            //Testing Parse Tree
            lexer.Reset();
            var parser = new MalinaParser(new CommonTokenStream(lexer));
            var malinaListener = new MalinaParserListener();
            var parserErrorListener = new ErrorListener<IToken>();
            parser.AddErrorListener(parserErrorListener);
            parser.AddParseListener(malinaListener);
            //parser.BuildParseTree = true;

            var module = parser.module();

            int nCount = 0;
            int tCount = 0;

            Console.WriteLine();
            Console.WriteLine("ParseTree:");
            Console.WriteLine();

            var sb = new StringBuilder();
            PrintTree(module, 0, sb, ref nCount, ref tCount);
            var parseTree = sb.ToString();
            
            var isParseTreeRecordedTest = IsParseTreeRecordedTest();
            var isParseTreeRecordTest = IsParseTreeRecordTest(); //Overwrites existing recording
            string recordedParseTree = null;
            if (isParseTreeRecordedTest || isParseTreeRecordTest)
            {
                if (isParseTreeRecordedTest) recordedParseTree = LoadRecordedParseTreeTest();
                if (recordedParseTree == null || isParseTreeRecordTest)
                {
                    SaveRecordedParseTreeTest(parseTree);
                }
            }

            Console.WriteLine(parseTree);

            Console.WriteLine();


            //Testing DOM generation
            Console.WriteLine("DOM:");
            Console.WriteLine();

            var printerVisitor = new DOMPrinterVisitor();
            //module.
            foreach (var item in malinaListener.Nodes)
            {
                printerVisitor.VisitNode(item);
            }

            Console.WriteLine(printerVisitor.Text);

            var isDomRecordedTest = IsDomRecordedTest();
            var isDomRecordTest = IsDomRecordTest(); //Overwrites existing recording
            string recordedDom = null;
            if (isDomRecordedTest || isDomRecordTest)
            {
                if (isDomRecordedTest) recordedDom = LoadRecordedDomTest();
                if (recordedDom == null || isDomRecordTest)
                {
                    SaveRecordedDomTest(printerVisitor.Text);
                }
            }


            //Lexer Assertions
            if (recorded != null)
            {
                Assert.AreEqual(recorded, printedTokens);
            }

            Assert.AreEqual(false, lexerErros.HasErrors);

            Assert.AreEqual(0, lexer.InvalidTokens.Count);

            //Parse Tree Assertions
            if (recordedParseTree != null)
            {
                Assert.AreEqual(recordedParseTree, parseTree);
            }

            //DOM Assertions
            if (recordedDom != null)
            {
                Assert.AreEqual(recordedDom, printerVisitor.Text.Replace("\r\n", "\n"));
            }

            //Assert.AreEqual(false, parserErrorListener.HasErrors);

        }

        private static IList<IToken> GetTokens(string code, out MalinaLexer lexer, out ErrorListener<int> lexerErros)
        {
            lexer = new MalinaLexer(new AntlrInputStream(code));
            lexerErros = new ErrorListener<int>();
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(lexerErros);

            return lexer.GetAllTokens();
        }

        private static bool IsLexerRecordedTest()
        {
            var trace = new StackTrace();
            var method = trace.GetFrames().Select(f => f.GetMethod()).Where(m => m.CustomAttributes.Any(a => a.AttributeType.Equals(typeof(TestAttribute)))).First();
            return method.CustomAttributes.Any(ca => ca.AttributeType.Equals(typeof(LexerRecordedAttribute))) ||
                method.DeclaringType.CustomAttributes.Any(ca => ca.AttributeType.Equals(typeof(LexerRecordedAttribute)));
        }

        private static bool IsLexerRecordTest()
        {
            var trace = new StackTrace();
            var method = trace.GetFrames().Select(f => f.GetMethod()).Where(m => m.CustomAttributes.Any(a => a.AttributeType.Equals(typeof(TestAttribute)))).First();
            return method.CustomAttributes.Any(ca => ca.AttributeType.Equals(typeof(LexerRecordAttribute))) ||
                method.DeclaringType.CustomAttributes.Any(ca => ca.AttributeType.Equals(typeof(LexerRecordAttribute)));
        }

        private static bool IsDomRecordedTest()
        {
            var trace = new StackTrace();
            var method = trace.GetFrames().Select(f => f.GetMethod()).Where(m => m.CustomAttributes.Any(a => a.AttributeType.Equals(typeof(TestAttribute)))).First();
            return method.CustomAttributes.Any(ca => ca.AttributeType.Equals(typeof(DomRecordedAttribute))) ||
                method.DeclaringType.CustomAttributes.Any(ca => ca.AttributeType.Equals(typeof(DomRecordedAttribute)));
        }

        private static bool IsDomRecordTest()
        {
            var trace = new StackTrace();
            var method = trace.GetFrames().Select(f => f.GetMethod()).Where(m => m.CustomAttributes.Any(a => a.AttributeType.Equals(typeof(TestAttribute)))).First();
            return method.CustomAttributes.Any(ca => ca.AttributeType.Equals(typeof(DomRecordAttribute))) ||
                method.DeclaringType.CustomAttributes.Any(ca => ca.AttributeType.Equals(typeof(DomRecordAttribute)));
        }

        private static bool IsParseTreeRecordedTest()
        {
            var trace = new StackTrace();
            var method = trace.GetFrames().Select(f => f.GetMethod()).Where(m => m.CustomAttributes.Any(a => a.AttributeType.Equals(typeof(TestAttribute)))).First();
            return method.CustomAttributes.Any(ca => ca.AttributeType.Equals(typeof(ParseTreeRecordedAttribute))) ||
                method.DeclaringType.CustomAttributes.Any(ca => ca.AttributeType.Equals(typeof(ParseTreeRecordedAttribute)));
        }

        private static bool IsParseTreeRecordTest()
        {
            var trace = new StackTrace();
            var method = trace.GetFrames().Select(f => f.GetMethod()).Where(m => m.CustomAttributes.Any(a => a.AttributeType.Equals(typeof(TestAttribute)))).First();
            return method.CustomAttributes.Any(ca => ca.AttributeType.Equals(typeof(ParseTreeRecordAttribute))) ||
                method.DeclaringType.CustomAttributes.Any(ca => ca.AttributeType.Equals(typeof(ParseTreeRecordAttribute)));
        }

        private static void PrintCode(string code)
        {
            int line = 1;
            Console.WriteLine("Code:");
            Console.Write("{0}:\t ", line);
            int offset = 0;
            foreach (var c in code)
            {
                if (c == '\n')
                {
                    Console.Write(" ({0})", offset);
                }

                Console.Write(c);
                offset++;
                if (c == '\n')
                {
                    line++;
                    Console.Write("{0}:\t ", line);
                }
            }
            Console.WriteLine();
        }

        public static string PrintedTokens(IEnumerable<IToken> tokens)
        {
            var sb = new StringBuilder();
            foreach (var token in tokens)
            {
                //string text = token.text ?? ;
                string text = token.Text;
                if (text != null)
                    text = text.Replace("\n", @"\n").Replace("\t", @"\t");
                
                var s = token.StartIndex;
                var e = token.StopIndex;
                sb.AppendLine(string.Format("\t{2}:{3}\t{0}\t`{1}`\t ({4},{5})", GetType(token), text, token.Line, token.Column, token.StartIndex, token.StopIndex));
            }
            return sb.ToString().Replace("\r\n", "\n");
        }

        private static string GetType(IToken token)
        {
            return (token.TokenSource as MalinaLexer).Vocabulary.GetSymbolicName(token.Type);
        }

        public static void PrintTree(ParserRuleContext ctx, int indent, StringBuilder sb, ref int nCount, ref int tCount)
        {
            nCount++;
            var symbols = new List<TerminalNodeImpl>();
            IEnumerable<ParserRuleContext> nodes = new List<ParserRuleContext>();
            if (ctx.children != null)
            {
                symbols = ctx.children.OfType<TerminalNodeImpl>().Where(
                    s => s.Symbol.Type != MalinaLexer.NEWLINE
                         && s.Symbol.Type != MalinaLexer.INDENT
                         && s.Symbol.Type != MalinaLexer.DEDENT
                         && !string.IsNullOrEmpty(s.Symbol.Text)
                    ).ToList();
                nodes = ctx.children.OfType<ParserRuleContext>();
            }
            tCount += symbols.Count();

            var ruleName = MalinaParser.ruleNames[ctx.RuleIndex];

            var sSymbols = string.Join(", ", symbols.Select(s => string.Format("{0}={1}", GetType(s.Symbol), s.Symbol.Text)));

            sb.Append('\n');
            sb.Append(string.Format("{0}{1}: ({2})", "".PadLeft(indent * 4), ruleName, sSymbols));

            foreach (var child in nodes)
            {
                PrintTree(child, indent + 1, sb, ref nCount, ref tCount);
            }
        }
    }
}
