using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malina.Compiler
{
    public static class ErrorCodes
    {
        public const string MCE0000 = "'{0}'";
        public const string MCE0001 = "Error reading from '{0}': '{1}'.";
        public const string MCE0002 = "File '{0}' was not found.";
        public const string MCE0003 = "Namespace prefix '{0}' is not defined.";
        public const string MCE0004 = "Alias '{0}' is not defined.";
        public const string MCE0005 = "Alias Definition '{0}' has circular reference.";
        public const string MCE0006 = "LexerError - '{0}'.";
        public const string MCE0007 = "ParserError - '{0}'.";

        public static string Format(string name, params object[] args)
        {
            return string.Format(GetString(name), args);
        }

        private static string GetString(string name)
        {
            return (string)typeof(ErrorCodes).GetField(name).GetValue(null);
        }
    }
}
