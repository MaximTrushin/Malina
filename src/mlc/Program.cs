using System.Collections.Generic;
using Malina.Compiler;
using Malina.Compiler.Pipelines;

namespace mlc
{
    class Program
    {
        public static int Main(string[] args)
        {
            var result = 1;
            try
            {
                var compilerParameters = new CompilerParameters {Pipeline = new CompileToFiles()};
                bool recursive;
                List<string> files;
                ArgumentsParser.Parse(args, out files, out recursive);

                var compiler = new MalinaCompiler(compilerParameters);
                compiler.Run();
                result = 0;
            }
            catch (ArgumentsParserException e)
            {
                System.Console.WriteLine("Fatal error: {0}", e.Message);
                ArgumentsParser.Help();

            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Fatal error: {0}", e.Message);    
            }
            System.Console.Read();
            return result;
        }
    }

}
