using Malina.Compiler;
using Malina.Compiler.Pipelines;

namespace Malina.Console
{
    class Program
    {
        public static int Main(string[] args)
        {
            int result = 1;
            try
            {
                var compilerParameters = new CompilerParameters();
                compilerParameters.Pipeline = new CompileToFiles();
                new ArgumentsParser(args, compilerParameters);

                var compiler = new MalinaCompiler(compilerParameters);
                compiler.Run();
                result = 0;
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(string.Format("Fatal error: {0}", e.Message));    
            }

            return result;
        }
    }

}
