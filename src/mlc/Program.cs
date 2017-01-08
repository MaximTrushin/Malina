using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Malina.Compiler;
using Malina.Compiler.IO;
using Malina.Compiler.Pipelines;

namespace mlc
{
    class Program
    {
        public static int Main(string[] args)
        {
            AppInfo();
            var result = 1;
            try
            {
                bool recursive;
                List<string> files;
                string outputDirectory;
                ArgumentsParser.Parse(args, out files, out recursive, out outputDirectory);

                var compilerParameters = GetCompilerParameters(files, outputDirectory);// new CompilerParameters { Pipeline = new CompileToFiles() };

                var compiler = new MalinaCompiler(compilerParameters);

                var context = compiler.Run();

                if (context.Errors.Count > 0)
                {
                    PrintCompilerErrors(context.Errors);
                }

                result = 0;
            }
            catch (ArgumentsParserException e)
            {
                Console.WriteLine("Fatal error: {0}", e.Message);
                ArgumentsParser.Help();

            }
            catch (Exception e)
            {
                Console.WriteLine("Fatal error: {0}", e.Message);
                Console.WriteLine(e.StackTrace);
            }
            Console.Read();
            return result;
        }

        private static CompilerParameters GetCompilerParameters(List<string> files, string outputDirectory)
        {
            var compilerParameters = new CompilerParameters {OutputDirectory = outputDirectory != string.Empty?outputDirectory: Directory.GetCurrentDirectory(), Pipeline = new CompileToFiles() };

            foreach (var fileName in files)
            {
                if (fileName.EndsWith(".mlx") || fileName.EndsWith(".mlj"))
                {
                    compilerParameters.Input.Add(new FileInput(fileName));
                    continue;
                }
                if (fileName.EndsWith(".xsd")) compilerParameters.XmlSchemaSet.Add(null, fileName);
            }
            return compilerParameters;
        }

        private static void AppInfo()
        {
            var type = typeof(MalinaCompiler);
            Console.WriteLine("Malina Compiler version {0}. Environment: {1}",
                type.Assembly.GetName().Version, RuntimeDisplayName);
        }

        public static string RuntimeDisplayName
        {
            get
            {
                var monoRuntime = Type.GetType("Mono.Runtime");
                return (monoRuntime != null)
                    ? (string)monoRuntime.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null)
                    : $"CLR {Environment.Version}";
            }
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

    }

}
