using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malina.Compiler
{
    public static class ErrorCodes
    {
        public const string MCE0000 = "Fatal error - '{0}'";
        public const string MCE0001 = "Error reading from '{0}': '{1}'.";
        public const string MCE0002 = "File '{0}' was not found.";
        public const string MCE0003 = "Namespace prefix '{0}' is not defined.";
        public const string MCE0004 = "Alias '{0}' is not defined.";
        public const string MCE0005 = "Alias Definition '{0}' has circular reference.";
        public const string MCE0006 = "LexerError - '{0}'.";
        public const string MCE0007 = "ParserError - '{0}'.";
        public const string MCE0008 = "Duplicate document name - '{0}'.";
        public const string MCE0009 = "Document '{0}' must have{1} one root element.";
        public const string MCE0010 = "Parameters can't be declared in documents.";
        public const string MCE0011 = "Duplicate argument name - '{0}'.";
        public const string MCE0012 = "Duplicate alias definition name - '{0}'.";
        public const string MCE0013 = "Argument '{0}' is missing.";
        public const string MCE0014 = "Value argument is expected.";
        public const string MCE0015 = "Block argument is expected.";
        public const string MCE0016 = "Can not use value alias in the block.";
        public const string MCE0017 = "Can not use block alias as value.";


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
