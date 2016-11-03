using Malina.Compiler;
using System.Collections.Generic;
using System.IO;

namespace Malina.Console
{

    public class ArgumentsParser
    {
        private static class ParameterErrors
        {
            public const string InvalidDirectory = "Invalid directory. Directory doesn't exist.";
            public const string InvalidFile = "Invalid file name. File doesn't exist.";
        }


        private CompilerParameters _compilerParameters;
        public ArgumentsParser(string[] args, CompilerParameters compilerParameters)
        {
            _compilerParameters = compilerParameters;

            foreach (var arg in args)
            {
                if (!IsFlag(arg))
                {
                    if (File.Exists(arg))
                        _compilerParameters.Files.Add(arg);
                    else InvalidOption(arg, ParameterErrors.InvalidFile);
                    continue;
                }

                switch (arg[1])
                {
                    case 'o':
                        {
                            if (arg[2] == '=')
                            {
                                var dir = arg.Substring(3);
                                if (Directory.Exists(dir))
                                    _compilerParameters.OutputDirectory = dir;
                                else InvalidOption(arg, ParameterErrors.InvalidDirectory);

                            }
                            else InvalidOption(arg);
                            break;
                        }
                }
            }
        }


        static bool IsFlag(string arg)
        {
            return arg[0] == '-';
        }

        static void Help()
        {
            System.Console.WriteLine(
                    "Usage: mlc [options] [inputFiles] ...\n" +
                    "Options:\n" +
                    " -o=DIR           Output directory\n" 
                    );
        }


        void InvalidOption(string arg)
        {
            InvalidOption(arg, null);
        }

        void InvalidOption(string arg, string message)
        {
            System.Console.Error.WriteLine("Invalid command line argument: {0}. {1}", arg, message);
        }

    }
}
