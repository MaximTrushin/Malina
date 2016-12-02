using NUnit.Framework;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Malina.Compiler;
using Malina.Compiler.Pipelines;
using Malina.Compiler.IO;
using static Malina.Parser.Tests.TestUtils;
using System;
using Malina.Parser.Tests;
using System.Text;
using Malina.DOM;
using System.Collections.Generic;

namespace Malina.Compiler.Tests
{
    public class TestUtils
    {
        public static void PerformCompilerTest(List<CompilerError> errors = null)
        {
            PrintTestScenario();

            var compilerParameters = CreateCompilerParameters();
            var compiler = new MalinaCompiler(compilerParameters);

            var context = compiler.Run();

            if (context.Errors.Count > 0)
            {
                PrintCompilerErrors(context.Errors);
            }

            if (errors == null)
                Assert.AreEqual(0, context.Errors.Count, "Compilation Errors Number is {0}", context.Errors.Count);
            else
            {
                Assert.AreEqual(errors.Count, context.Errors.Count, "Expected Errors Number is {0}", errors.Count);
                var i = 0;
                foreach (var contextError in context.Errors)
                {
                    CompareErrors(errors[i++], contextError);
                }
            }



            var isDomRecordedTest = IsDomRecordedTest();
            var isDomRecordTest = IsDomRecordTest(); //Overwrites existing recording
            string recordedDom = null;
            if (isDomRecordedTest || isDomRecordTest)
            {
                var printerVisitor = new DOMPrinterVisitor();
                printerVisitor.Visit(context.CompileUnit);
                Console.WriteLine();
                Console.WriteLine(printerVisitor.Text);

                if (isDomRecordedTest) recordedDom = LoadRecordedDomTest(true);
                if (recordedDom == null || isDomRecordTest)
                {
                    SaveRecordedDomTest(printerVisitor.Text, true);
                }

                //DOM Assertions
                if (recordedDom != null)
                {
                    Assert.AreEqual(recordedDom, printerVisitor.Text.Replace("\r\n", "\n"), "DOM assertion failed");
                }

            }



            if (IsRecordedTest() || IsRecordTest())
                CompareResultAndRecordedFiles(IsRecordTest());

        }

        private static void CompareErrors(CompilerError compilerError1, CompilerError compilerError2)
        {
            Assert.AreEqual(Path.GetFileName(compilerError1.LexicalInfo.FileName), Path.GetFileName(compilerError2.LexicalInfo.FileName));
            Assert.AreEqual(compilerError1.LexicalInfo.Line, compilerError2.LexicalInfo.Line);
            Assert.AreEqual(compilerError1.LexicalInfo.Column, compilerError2.LexicalInfo.Column);


            Assert.AreEqual(compilerError1.Message, compilerError2.Message);
        }

        public static void PrintCompilerErrors(IEnumerable<CompilerError> errors)
        {
            Console.WriteLine("Compiler Errors:");

            foreach (var error in errors)
            {
                Console.WriteLine();
                Console.Write(error.Code + " " + error.LexicalInfo + ": ");
                Console.WriteLine(error.Message);
                if (error.InnerException != null)
                    Console.WriteLine(error.InnerException.StackTrace);
            }
        }

        /// <summary>
        /// Record result file or compare it with previously recorded result
        /// </summary>
        /// <param name="record">If true then result file is recorded not compared.</param>
        private static void CompareResultAndRecordedFiles(bool record)
        {
            var testCaseName = GetTestCaseName();
            var resultDir = AssemblyDirectory + '\\' + "Scenarios" + '\\' + testCaseName + "\\Result\\";
            var recordedDir = AssemblyDirectory + @"\Scenarios\Recorded\" + testCaseName + @"\Compiler\";

            if (record)
            {
                var recordDir = AssemblyDirectory + @"\..\..\Scenarios\Recorded\" + testCaseName + @"\Compiler\";
                if(Directory.Exists(recordDir)) Directory.Delete(recordDir, true);
                Directory.CreateDirectory(recordDir);
                foreach(var file in Directory.GetFiles(resultDir))
                {
                    var newFile = recordDir + Path.GetFileName(file);
                    File.Copy(file, newFile, true);
                }               
            }
            else
            {
                Assert.IsTrue(Directory.Exists(resultDir), "Directory {0} doesn't exist", resultDir);
                Assert.IsTrue(Directory.Exists(recordedDir), "Directory {0} doesn't exist", recordedDir);

                //Equal number of files
                Assert.AreEqual(Directory.GetFiles(recordedDir).Count(), Directory.GetFiles(resultDir).Count(), "Number of files {0} in '{1}' should be equal {2}", Directory.GetFiles(resultDir).Count(), resultDir, Directory.GetFiles(recordedDir).Count());
                Console.WriteLine();
                Console.WriteLine("Generated Files:");
                foreach (var file in Directory.GetFiles(recordedDir))
                {
                    var recordedFileName = Path.GetFileName(file);
                    var resultFileName = resultDir + recordedFileName;
                    var result = File.ReadAllText(resultFileName).Replace("\r\n", "\n");
                    var recorded = File.ReadAllText(file).Replace("\r\n", "\n");

                    Console.WriteLine(string.Format("File {0}:", file));
                    Console.WriteLine(result);
                    Assert.AreEqual(recorded, result);
                }
            }
            Directory.Delete(resultDir, true);

        }

        public static CompilerContext PerformProcessAliasesTest()
        {
            PrintTestScenario();

            var compilerParameters = CreateCompilerParameters();
            var compiler = new MalinaCompiler(compilerParameters);

            var context = compiler.Run();
            var printerVisitor = new DOMPrinterVisitor();
            printerVisitor.Visit(context.CompileUnit);
            Console.WriteLine();
            Console.WriteLine(printerVisitor.Text);

            var isDomRecordedTest = IsDomRecordedTest();
            var isDomRecordTest = IsDomRecordTest(); //Overwrites existing recording
            string recordedDom = null;
            if (isDomRecordedTest || isDomRecordTest)
            {
                if (isDomRecordedTest) recordedDom = LoadRecordedDomTest(true);
                if (recordedDom == null || isDomRecordTest)
                {
                    SaveRecordedDomTest(printerVisitor.Text, true);
                }
            }

            //DOM Assertions
            if (recordedDom != null)
            {
                Assert.AreEqual(recordedDom, printerVisitor.Text.Replace("\r\n", "\n"), "DOM assertion failed");
            }

            return context;

        }

        private static void PrintTestScenario()
        {
            var testCaseName = GetTestCaseName();

            var dir = new StringBuilder(AssemblyDirectory + @"\Scenarios\").Append(testCaseName).ToString();

            foreach (var fileName in Directory.EnumerateFiles(dir))
            {
                if (fileName.EndsWith(".mlx"))
                {
                    Console.WriteLine();
                    Console.WriteLine(Path.GetFileName(fileName));
                    var code = File.ReadAllText(fileName);
                    PrintCode(code);
                }
            }
        }

        private static CompilerParameters CreateCompilerParameters()
        {
            var compilerParameters = new CompilerParameters();
            compilerParameters.Pipeline = new CompileToFiles();

            var dir = AssemblyDirectory + '\\' + "Scenarios" + '\\' + GetTestCaseName() + '\\';

            compilerParameters.OutputDirectory = dir + "Result" + '\\';

            foreach (var fileName in Directory.EnumerateFiles(dir))
            {
                if (fileName.EndsWith(".mlx"))
                {
                    compilerParameters.Input.Add(new FileInput(fileName));
                }
                if (fileName.EndsWith(".xsd")) compilerParameters.XmlSchemaSet.Add(null, fileName);
            } 

            return compilerParameters;
        }

        public static string AssemblyDirectory
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }
 
        private static string GetTestCaseName()
        {
            var trace = new StackTrace();
            return trace.GetFrames().Select(f => f.GetMethod()).Where(m => m.CustomAttributes.Any(a => a.AttributeType.FullName == "NUnit.Framework.TestAttribute")).First().Name;
        }

        private static bool TestHasAttribute<T>()
        {
            var trace = new StackTrace();
            var method = trace.GetFrames().Select(f => f.GetMethod()).Where(m => m.CustomAttributes.Any(a => a.AttributeType.Equals(typeof(TestAttribute)))).First();
            return method.CustomAttributes.Any(ca => ca.AttributeType.Equals(typeof(T))) ||
                method.DeclaringType.CustomAttributes.Any(ca => ca.AttributeType.Equals(typeof(T)));
        }

        public static bool IsRecordedTest()
        {
            return TestHasAttribute<RecordedTestAttribute>();
        }

        public static bool IsRecordTest()
        {
            return TestHasAttribute<RecordTestAttribute>();
        }


    }
}
