using System;
using System.Net.Configuration;
using Kermit.Parser.Exceptions;

namespace Kermit.Parser
{
    internal static class ThrowHelper
    {
        internal static void AlreadyDefinedArg(string source, int line, string input, string varname, Exception inner)
        {
            string msg = "Argument " + varname + " already defined";
            SyntaxError(source, line, 0, input, msg, inner);
        }

        internal static void SyntaxError(string source, int line, int position, string input, string message,
            Exception inner)
        {
            string msg =
                $"File: {source} at line {line}:{position}\n{input}\nSyntaxError: {message}";
            throw new ParserException(msg, inner);
        }
    }
}
