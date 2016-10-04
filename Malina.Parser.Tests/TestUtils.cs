using Antlr4.Runtime;
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
            return LoadTestCode(3);
        }

        public static string LoadTestCode(int index)
        {
            var trace = new StackTrace();
            var method = trace.GetFrame(index).GetMethod();
            var testCaseName = trace.GetFrame(index).GetMethod().Name;
            var fileName = new StringBuilder(AssemblyDirectory + @"\Scenarios\Lexer\").Append(testCaseName).Append(".mal").ToString();

            return File.ReadAllText(fileName).Replace("\r\n", "\n");
        }

        public static void PerformTest()
        {
            var code = LoadTestCode();

            PrintCode(code);

            var lexer = new MalinaLexer(new AntlrInputStream(code));
            var lexerErros = new ErrorListener<int>();
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(lexerErros);            

            var tokens = lexer.GetAllTokens();
            PrintTokens(tokens);
            Assert.AreEqual(false, lexerErros.HasErrors);
            Assert.AreEqual(0, lexer.Invalid.Count);
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

        public static void PrintTokens(IEnumerable<IToken> tokens)
        {
            Console.WriteLine("");
            Console.WriteLine("Tokens:");
            foreach (var token in tokens)
            {
                //string text = token.text ?? ;
                string text = token.Text;
                if (text != null)
                    text = text.Replace("\n", @"\n").Replace("\t", @"\t");
                
                var s = token.StartIndex;
                var e = token.StopIndex;
                Console.WriteLine("\t{2}:{3}\t{0} {1}\t ({4},{5})", GetType(token), text, token.Line, token.Column, token.StartIndex, token.StopIndex);
            }
        }

        private static string GetType(IToken token)
        {
            return (token.TokenSource as MalinaLexer).Vocabulary.GetSymbolicName(token.Type);
        }
    }
}
