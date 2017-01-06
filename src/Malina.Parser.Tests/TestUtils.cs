using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Tree;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Malina.Parser.Tests
{
    public class TestUtils
    {
        public static string AssemblyDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string LoadTestCode()
        {
            var testCaseName = GetTestCaseName();

            var fileName = AssemblyDirectory + @"\Scenarios\"+ testCaseName;

            if (File.Exists(fileName + ".mlj")) fileName += ".mlj";
            else fileName += ".mlx";

            return File.ReadAllText(fileName).Replace("\r\n", "\n");
        }

        public static string LoadTestCodeRaw()
        {
            var testCaseName = GetTestCaseName();

            var fileName = new StringBuilder(AssemblyDirectory + @"\Scenarios\").Append(testCaseName).Append(".mlx").ToString();

            return File.ReadAllText(fileName);
        }

        public static TextReader GetTestCodeReader()
        {
            var testCaseName = GetTestCaseName();

            var fileName = new StringBuilder(AssemblyDirectory + @"\Scenarios\").Append(testCaseName).Append(".mlx").ToString();

            return File.OpenText(fileName);
        }


        private static string GetTestCaseName()
        {
            var trace = new StackTrace();
            return trace.GetFrames().Select(f => f.GetMethod()).First(m => m.CustomAttributes.Any(a => a.AttributeType.FullName == "NUnit.Framework.TestAttribute")).Name;
        }

        public static string LoadRecordedTest(string printedTokens)
        {
            var isLexerRecordedTest = IsLexerRecordedTest();
            var isLexerRecordTest = IsLexerRecordTest(); //Overwrites existing recording
            string recorded = null;
            if (isLexerRecordedTest || isLexerRecordTest)
            {
                if (isLexerRecordedTest) {

                    var testCaseName = GetTestCaseName();
                    var fileName = new StringBuilder(AssemblyDirectory + @"\Scenarios\Recorded\").Append(testCaseName).Append(".test").ToString();
                    if (File.Exists(fileName))
                        recorded = File.ReadAllText(fileName).Replace("\r\n", "\n");
                }
                if (recorded == null || isLexerRecordTest)
                {
                    SaveRecordedTest(printedTokens);
                }
            }            
            return recorded;
        }

        /// <summary>
        /// Returns recorded lexer errors as result. Saves lexer errors for the test case if the test case has 
        /// attributes LexerErrorRecorded or LexerErrorRecord.
        /// Overwrites test recording is test has attribute LexerErrorRecorded.
        /// </summary>
        /// <param name="lexerErrors">Lexer error listener having all the errors.</param>
        /// <param name="serialLexerErrors">Returns serialized string representation of errors stored in lexerErrors</param>
        /// <returns></returns>
        public static string LoadLexerErrors(ErrorListener<int> lexerErrors, out string serialLexerErrors)
        {
            var isLexerErrorRecordedTest = IsLexerErrorRecordedTest();
            var isLexerErrorRecordTest = IsLexerErrorRecordTest(); //Overwrites existing recording
            
            serialLexerErrors = null;

            if (!isLexerErrorRecordedTest && !isLexerErrorRecordTest) return null;

            string recorded = null;
            serialLexerErrors = lexerErrors.Errors.Count > 0 ? SerializeErrors(lexerErrors.Errors) : null;
            if (isLexerErrorRecordedTest)
            {
                var testCaseName = GetTestCaseName();
                var fileName = new StringBuilder(AssemblyDirectory + @"\Scenarios\Recorded\").Append(testCaseName).Append(".le").ToString();
                if (File.Exists(fileName))
                    recorded = File.ReadAllText(fileName).Replace("\r\n", "\n");
            }
            if (recorded == null || isLexerErrorRecordTest)
            {
                SaveLexerErrors(serialLexerErrors);
                return serialLexerErrors;
            }
            return recorded;
        }

        public static string LoadParserErrors(ErrorListener<IToken> parserErrors, out string serialParserErrors)
        {
            var isParserErrorRecordedTest = IsParserErrorRecordedTest();
            var isParserErrorRecordTest = IsParserErrorRecordTest(); //Overwrites existing recording
            string recorded = null;
            serialParserErrors = null;
            if (isParserErrorRecordedTest || isParserErrorRecordTest)
            {
                serialParserErrors = parserErrors.Errors.Count > 0 ? SerializeErrors(parserErrors.Errors) : null;
                if (isParserErrorRecordedTest)
                {
                    var testCaseName = GetTestCaseName();
                    var fileName = new StringBuilder(AssemblyDirectory + @"\Scenarios\Recorded\").Append(testCaseName).Append(".le").ToString();
                    if (File.Exists(fileName))
                        recorded = File.ReadAllText(fileName).Replace("\r\n", "\n");
                }
                if (recorded == null || isParserErrorRecordTest)
                {
                    SaveLexerErrors(serialParserErrors);
                    return serialParserErrors;
                }
            }
            return recorded;
        }


        private static void SaveRecordedTest(string printedTokens)
        {
            var testCaseName = GetTestCaseName();
            var fileName = new StringBuilder(AssemblyDirectory + @"\..\..\Scenarios\Recorded\").Append(testCaseName).Append(".test").ToString();
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            File.WriteAllText(fileName, printedTokens);
        }

        private static void SaveLexerErrors(string serialLexerErrors)
        {
            var testCaseName = GetTestCaseName();
            var fileName = new StringBuilder(AssemblyDirectory + @"\..\..\Scenarios\Recorded\").Append(testCaseName).Append(".le").ToString();
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            File.WriteAllText(fileName, serialLexerErrors);
        }

        public static string LoadRecordedParseTreeTest()
        {
            var testCaseName = GetTestCaseName();
            var fileName = new StringBuilder(AssemblyDirectory + @"\Scenarios\Recorded\").Append(testCaseName).Append(".tree").ToString();
            if (!File.Exists(fileName)) return null;

            return File.ReadAllText(fileName).Replace("\r\n", "\n");
        }
        
        private static void SaveRecordedParseTreeTest(string parseTree)
        {
            var testCaseName = GetTestCaseName();
            var fileName = new StringBuilder(AssemblyDirectory + @"\..\..\Scenarios\Recorded\").Append(testCaseName).Append(".tree").ToString();
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            File.WriteAllText(fileName, parseTree);
        }

        public static string LoadRecordedDomTest(bool scenarioHasFolder = false)
        {
            var testCaseName = GetTestCaseName();
            var sb = new StringBuilder(AssemblyDirectory + @"\Scenarios\Recorded\").Append(testCaseName);
            if (scenarioHasFolder) sb.Append('\\').Append(testCaseName);
            sb.Append(".dom");

            var fileName = sb.ToString();
            if (!File.Exists(fileName)) return null;

            return File.ReadAllText(fileName).Replace("\r\n", "\n");
        }

        public static void SaveRecordedDomTest(string printedTokens, bool scenarioHasFolder = false)
        {
            var testCaseName = GetTestCaseName();
            var sb = new StringBuilder(AssemblyDirectory + @"\..\..\Scenarios\Recorded\").Append(testCaseName);
            if (scenarioHasFolder) sb.Append('\\').Append(testCaseName);
            sb.Append(".dom");
            var fileName = sb.ToString();
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            File.WriteAllText(fileName, printedTokens);
        }


        private static string SerializeErrors(List<MalinaException> errors)
        {
            var sb = new StringBuilder();
            foreach (var item in errors)
            {
                sb.AppendLine(item.ToString());
            }

            return sb.ToString().Replace("\r\n", "\n");

        }
        public static void PerformTest()
        {
            var code = LoadTestCode();

            PrintCode(code);

            //Testing Lexer
            MalinaLexer lexer;
            ErrorListener<int> lexerErrors;
            IList<IToken> tokens =  GetTokens(code, out lexer, out lexerErrors);

            var printedTokens = PrintedTokens(tokens);

            Console.WriteLine("");
            Console.WriteLine("Tokens:");
            Console.WriteLine(printedTokens);


            string recorded = LoadRecordedTest(printedTokens);
            string serialLexerErrors;
            var recordedLexerErros = LoadLexerErrors(lexerErrors, out serialLexerErrors);
            if (recordedLexerErros != null)
            {
                Console.WriteLine("Lexer Errors:");
                Console.WriteLine(recordedLexerErros);
            }

            if (lexerErrors.HasErrors)
                PrintErrors(lexerErrors.Errors, "Lexer Errors: ");

            //Testing Parse Tree
            lexer.Reset();
            lexer.ErrorListeners.Clear();
            var parser = MalinaParser.Create(new CommonTokenStream(lexer));
            parser.Interpreter.PredictionMode = PredictionMode.Sll;
            var malinaListener = new MalinaParserListener();
            var parserErrorListener = new ErrorListener<IToken>();
            parser.AddErrorListener(parserErrorListener);
            parser.AddParseListener(malinaListener);

            //parser.AddErrorListener(new DiagnosticErrorListener());
            //parser.Interpreter.PredictionMode = PredictionMode.LlExactAmbigDetection;

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

            //Parser Errors
            string serialParserErrors;
            var recordedParserErros = LoadParserErrors(parserErrorListener, out serialParserErrors);
            if (recordedParserErros != null)
            {
                Console.WriteLine("Parser Errors:");
                Console.WriteLine(recordedParserErros);
            }

            Console.WriteLine(parseTree);

            Console.WriteLine();


            //Testing DOM generation
            Console.WriteLine("DOM:");
            Console.WriteLine();

            var printerVisitor = new DOMPrinterVisitor();
            //module.

            printerVisitor.Visit(malinaListener.CompileUnit);


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

            PrintCode(code);

            //LEXER Assertions
            if (recordedLexerErros != null)
            {
                Assert.AreEqual(recordedLexerErros, serialLexerErrors);
            }
            else
            {
                Assert.AreEqual(false, lexerErrors.HasErrors, "LexerErrorListener has errors");
            }
                

            Assert.AreEqual(0, lexer.InvalidTokens.Count);

            if (recorded != null)
            {
                Assert.AreEqual(recorded, printedTokens, "LEXER assertion failed");
            }

            //Parse Tree Assertions
            if (recordedParseTree != null)
            {
                Assert.AreEqual(recordedParseTree, parseTree, "PARSE TREE assertion failed");
            }

            if (recordedParserErros != null)
            {
                Assert.AreEqual(recordedParserErros, serialParserErrors);
            }
            else
            {
                if (parserErrorListener.HasErrors)
                    PrintErrors(parserErrorListener.Errors, "Parser Errors:");       
                
                Assert.AreEqual(false, parserErrorListener.HasErrors, "ParserErrorListener has errors");
            }
                

            //DOM Assertions
            if (recordedDom != null)
            {
                Assert.AreEqual(recordedDom, printerVisitor.Text.Replace("\r\n", "\n"), "DOM assertion failed");
            }

        }

        public static void PrintErrors(List<MalinaException> errors, string title)
        {
            Console.WriteLine(title);

            foreach (var error in errors)
            {
                Console.WriteLine();
                Console.Write(error.Code + " " + error.Start + "-" + error.Stop + ": ");
                Console.WriteLine(error.Message);
                if (error.InnerException != null)
                    Console.WriteLine(error.InnerException.StackTrace);
            }
        }

        private static IList<IToken> GetTokens(string code, out MalinaLexer lexer, out ErrorListener<int> lexerErros)
        {
            lexer = new MalinaLexer(new AntlrInputStream(code));
            lexerErros = new ErrorListener<int>();
            //lexer.RemoveErrorListeners();
            lexer.AddErrorListener(lexerErros);

            return lexer.GetAllTokens();
        }

        private static bool IsLexerRecordedTest()
        {
            return TestHasAttribute<LexerRecordedAttribute>();
        }

        private static bool IsLexerRecordTest()
        {
            return TestHasAttribute<LexerRecordAttribute>();
        }

        private static bool IsLexerErrorRecordedTest()
        {
            return TestHasAttribute<LexerErrorRecordedAttribute>();
        }

        private static bool IsLexerErrorRecordTest()
        {
            return TestHasAttribute<LexerErrorRecordAttribute>();
        }

        private static bool IsParserErrorRecordedTest()
        {
            return TestHasAttribute<ParserErrorRecordedAttribute>();
        }

        private static bool IsParserErrorRecordTest()
        {
            return TestHasAttribute<ParserErrorRecordAttribute>();
        }

        public static bool IsDomRecordedTest()
        {
            return TestHasAttribute<DomRecordedAttribute>();
        }

        public static bool IsDomRecordTest()
        {
            return TestHasAttribute<DomRecordAttribute>();
        }

        private static bool IsParseTreeRecordedTest()
        {
            return TestHasAttribute<ParseTreeRecordedAttribute>();
        }

        private static bool IsParseTreeRecordTest()
        {
            return TestHasAttribute<ParseTreeRecordAttribute>();
        }



        private static bool TestHasAttribute<T>()
        {
            var trace = new StackTrace();
            var method = trace.GetFrames().Select(f => f.GetMethod()).First(m => m.CustomAttributes.Any(a => a.AttributeType == typeof(TestAttribute)));
            return method.CustomAttributes.Any(ca => ca.AttributeType == typeof(T)) ||
                method.DeclaringType.CustomAttributes.Any(ca => ca.AttributeType == typeof(T));
        }

        public static void PrintCode(string code)
        {
            int line = 1;
            Console.WriteLine("Code:");
            Console.Write("{0}:\t ", line);
            int offset = 0;
            foreach (var c in code)
            {
                if (c == '\r') continue;
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
            Console.Write(" ({0})", offset);
            Console.WriteLine();
        }

        public static string PrintedTokens(IEnumerable<IToken> tokens)
        {
            var sb = new StringBuilder();
            foreach (var token in tokens)
            {
                var text = token.StartIndex > -1? token.Text:"";
                text = text?.Replace("\n", @"\n").Replace("\t", @"\t");

                sb.AppendLine(string.Format("\t{2}:{3}\t{0}\t`{1}`\t ({4},{5})", GetType(token), text, token.Line, token.Column, token.StartIndex, token.StopIndex));
            }
            return sb.ToString().Replace("\r\n", "\n");
        }

        private static string GetType(IToken token)
        {
            return ((MalinaLexer) token.TokenSource).Vocabulary.GetSymbolicName(token.Type);
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
            tCount += symbols.Count;

            var ruleName = MalinaParser.ruleNames[ctx.RuleIndex];

            var sSymbols = string.Join(", ", symbols.Select(s => $"{GetType(s.Symbol)}={s.Symbol.Text}"));

            sb.Append('\n');
            sb.Append($"{"".PadLeft(indent*4)}{ruleName}: ({sSymbols})");

            foreach (var child in nodes)
            {
                PrintTree(child, indent + 1, sb, ref nCount, ref tCount);
            }
        }
    }
}
