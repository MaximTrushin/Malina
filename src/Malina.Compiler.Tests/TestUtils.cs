#region license
// Copyright © 2016, 2017 Maxim Trushin (dba Syntactik, trushin@gmail.com, maxim.trushin@syntactik.com)
//
// This file is part of Malina.
// Malina is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// Malina is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Malina.  If not, see <http://www.gnu.org/licenses/>.
#endregion
using NUnit.Framework;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Malina.Compiler.Pipelines;
using Malina.Compiler.IO;
using System;
using System.Text;
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



            if (IsRecordedTest() || IsRecordTest())
                CompareResultAndRecordedFiles(IsRecordTest());

        }

        private static void CompareErrors(CompilerError compilerError1, CompilerError compilerError2)
        {
            Assert.AreEqual(Path.GetFileName(compilerError1.LexicalInfo.FileName), Path.GetFileName(compilerError2.LexicalInfo.FileName));
            Assert.AreEqual(compilerError1.LexicalInfo.Line, compilerError2.LexicalInfo.Line);
            Assert.AreEqual(compilerError1.LexicalInfo.Column, compilerError2.LexicalInfo.Column, compilerError2.Message);


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
                Assert.AreEqual(Directory.GetFiles(recordedDir).Length, Directory.GetFiles(resultDir).Length, "Number of files {0} in '{1}' should be equal {2}", Directory.GetFiles(resultDir).Length, resultDir, Directory.GetFiles(recordedDir).Length);
                Console.WriteLine();
                Console.WriteLine("Generated Files:");
                foreach (var file in Directory.GetFiles(recordedDir))
                {
                    var recordedFileName = Path.GetFileName(file);
                    var resultFileName = resultDir + recordedFileName;
                    var result = File.ReadAllText(resultFileName).Replace("\r\n", "\n");
                    var recorded = File.ReadAllText(file).Replace("\r\n", "\n");

                    Console.WriteLine($"File {file}:");
                    Console.WriteLine(result);
                    Assert.AreEqual(recorded, result);
                }
            }
            Directory.Delete(resultDir, true);

        }

        private static void PrintTestScenario()
        {
            var testCaseName = GetTestCaseName();

            var dir = new StringBuilder(AssemblyDirectory + @"\Scenarios\").Append(testCaseName).ToString();

            foreach (var fileName in Directory.EnumerateFiles(dir))
            {
                if (!fileName.EndsWith(".mlx") && !fileName.EndsWith(".mlj")) continue;

                Console.WriteLine();
                Console.WriteLine(Path.GetFileName(fileName));
                var code = File.ReadAllText(fileName);
                PrintCode(code);
            }
        }

        private static CompilerParameters CreateCompilerParameters()
        {
            var compilerParameters = new CompilerParameters {Pipeline = new CompileToFiles()};

            var dir = AssemblyDirectory + '\\' + "Scenarios" + '\\' + GetTestCaseName() + '\\';

            compilerParameters.OutputDirectory = dir + "Result" + '\\';

            foreach (var fileName in Directory.EnumerateFiles(dir))
            {
                if (fileName.EndsWith(".mlx") || fileName.EndsWith(".mlj"))
                {
                    compilerParameters.Input.Add(new FileInput(fileName));
                }
                if (fileName.EndsWith(".xsd")) compilerParameters.XmlSchemaSet.Add(null, fileName);
            } 

            return compilerParameters;
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

        public static string AssemblyDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static string GetTestCaseName()
        {
            var trace = new StackTrace();
            return trace.GetFrames().Select(f => f.GetMethod()).First(m => m.CustomAttributes.Any(a => a.AttributeType.FullName == "NUnit.Framework.TestAttribute")).Name;
        }

        private static bool TestHasAttribute<T>()
        {
            var trace = new StackTrace();
            var method = trace.GetFrames().Select(f => f.GetMethod()).First(m => m.CustomAttributes.Any(a => a.AttributeType == typeof(TestAttribute)));
            Debug.Assert(method.DeclaringType != null, "method.DeclaringType != null");
            return method.CustomAttributes.Any(ca => ca.AttributeType == typeof(T)) ||
                method.DeclaringType.CustomAttributes.Any(ca => ca.AttributeType == typeof(T));
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
