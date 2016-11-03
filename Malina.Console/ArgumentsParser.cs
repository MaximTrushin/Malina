using Malina.Compiler;
using System;
using System.Collections.Generic;
using System.IO;

namespace Malina.Console
{

    public class ArgumentsParser
    {
        public static class ParameterErrors
        {
            public const string InvalidDirectory = "Invalid directory. Directory doesn't exist.";
            public const string InvalidFile = "Invalid file name. File doesn't exist.";
            public const string NoInput = "No input files or directories specified.";
        }

        public static void Parse(string[] args, CompilerParameters compilerParameters)
        {
            foreach (var arg in args)
            {
                if (!IsFlag(arg))
                {
                    if (File.Exists(arg))
                        compilerParameters.Files.Add(arg);
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
                                    compilerParameters.OutputDirectory = dir;
                                else InvalidOption(arg, ParameterErrors.InvalidDirectory);

                            }
                            else InvalidOption(arg);
                            break;
                        }
                }
            }
            if (compilerParameters.Files.Count == 0) throw new ArgumentsParserException(ParameterErrors.NoInput);
        }






        static bool IsFlag(string arg)
        {
            return arg[0] == '-';
        }

        public static void Help()
        {
            System.Console.WriteLine(
                    "Options: mlc [options] [inputFiles] ...\n\n" +
                    "Compiles a set of files in Malina format into XML or Json.\n\n" +
                    "You many specify one or many files of type .mlx and .mlj or directories.\n\n" +
                    "Options:\n" +
                    " -o=DIR           Output directory\n"
                    );
        }


        static void InvalidOption(string arg)
        {
            InvalidOption(arg, null);
        }

        static void InvalidOption(string arg, string message)
        {
            System.Console.Error.WriteLine("Invalid command line argument: {0}. {1}", arg, message);
        }

    }
}
