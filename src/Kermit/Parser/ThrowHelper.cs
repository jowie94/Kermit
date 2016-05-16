using System;
using Kermit.Parser.Exceptions;

namespace Kermit.Parser
{
    /// <summary>
    /// Helper function used for throwing exceptions
    /// </summary>
    internal static class ThrowHelper
    {
        /// <summary>
        /// Error when an argument name is already defined
        /// </summary>
        /// <param name="source">Source file</param>
        /// <param name="line">Line where the error happened</param>
        /// <param name="input">Current input</param>
        /// <param name="varname">Variable name</param>
        /// <param name="inner">Inner exception</param>
        internal static ParserException AlreadyDefinedArg(string source, int line, string input, string varname, Exception inner)
        {
            string msg = "Argument " + varname + " already defined";
            return SyntaxError(source, line, 0, input, msg, inner);
        }

        /// <summary>
        /// Syntax error while parsing
        /// </summary>
        /// <param name="source">Source file</param>
        /// <param name="line">Line where the error happened</param>
        /// <param name="position">Position where the error happened</param>
        /// <param name="input">Current input</param>
        /// <param name="message">Error message</param>
        /// <param name="inner">Inner exception</param>
        internal static ParserException SyntaxError(string source, int line, int position, string input, string message,
            Exception inner)
        {
            string msg =
                $"File: {source} at line {line}:{position}\n{input}\nSyntaxError: {message}";
            return new ParserException(msg, inner);
        }
    }
}
