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

namespace Malina.Compiler.Tests
{
    public class TestUtils
    {
        public static void PerformTest()
        {
            var compilerParameters = CreateCompilerParameters();
            var compiler = new MalinaCompiler(compilerParameters);

            var context = compiler.Run();

        }

        public static void PerformProcessAliasesTest()
        {
            PrintTestScenario();

            var compilerParameters = CreateCompilerParameters();
            var compiler = new MalinaCompiler(compilerParameters);

            var context = compiler.Run();
            var printerVisitor = new DOMPrinterVisitor();
            printerVisitor.VisitNode(context.CompileUnit);
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

        }

        private static void PrintTestScenario()
        {
            var testCaseName = GetTestCaseName();

            var dir = new StringBuilder(AssemblyDirectory + @"\Scenarios\").Append(testCaseName).ToString();

            foreach (var fileName in Directory.EnumerateFiles(dir))
            {
                if (fileName.EndsWith(".mlx"))
                {
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

            compilerParameters.OutputDirectory = dir + "Recorded" + '\\';

            foreach (var fileName in Directory.EnumerateFiles(dir))
            {
                if (fileName.EndsWith(".mlx"))
                {
                    compilerParameters.Input.Add(new FileInput(fileName));
                }
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
  
    }
}
