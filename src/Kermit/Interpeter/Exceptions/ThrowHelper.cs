using System;

namespace Kermit.Interpeter.Exceptions
{
    /// <summary>
    /// Class to help throwing exceptions
    /// </summary>
    internal static class ThrowHelper
    {
        /// <summary>
        /// Base exception, throws <see cref="InterpreterException(string,string[])"/>
        /// </summary>
        /// <param name="message">Message of the exception</param>
        /// <param name="stack">The call stack</param>
        internal static void InterpreterException(string message, string[] stack = null)
        {
            throw new InterpreterException(message, stack);
        }

        /// <summary>
        /// Base exception, throws <see cref="InterpreterException(string,Exception,string[])"/>
        /// </summary>
        /// <param name="message">Message of the exception</param>
        /// <param name="inner">Inner exception</param>
        /// <param name="stack">The call stack</param>
        internal static void InterpreterException(string message, Exception inner, string[] stack = null)
        {
            throw new InterpreterException(message, inner, stack);
        }

        /// <summary>
        /// Thrown when a symnol is not found
        /// </summary>
        /// <param name="name">The name of the symbol</param>
        /// <param name="stack">The call stack</param>
        internal static void NameNotExists(string name, string[] stack = null)
        {
            NameError("name '" + name + "' is not defined", stack);
        }

        /// <summary>
        /// General Name errors exception
        /// </summary>
        /// <param name="message">Message of the error</param>
        /// <param name="stack">The call stack</param>
        internal static void NameError(string message, string[] stack = null)
        {
            InterpreterException("NameError: " + message, stack);
        }

        /// <summary>
        /// Thrown when trying to perform illegal operations on a type
        /// </summary>
        /// <param name="message">Message of the error</param>
        /// <param name="stack">The call stack</param>
        internal static void TypeError(string message, string[] stack = null)
        {
            InterpreterException("TypeError: " + message, stack);
        }

        /// <summary>
        /// Thrown when a native function throws an exception
        /// </summary>
        /// <param name="name">Name of the function</param>
        /// <param name="e">Exception thrown</param>
        internal static void NativeFunctionError(string name, Exception e)
        {
            InterpreterException($"[NE] {e.GetType().Name} thrown by {name}:\n{e.Message}", e);
        }

        /// <summary>
        /// Thrown when there are errors with function attributes
        /// </summary>
        /// <param name="message">Message of the error</param>
        /// <param name="stack">The call stack</param>
        internal static void AttributeError(string message, string[] stack = null)
        {
            InterpreterException("AttributeError: " + message, stack);
        }

        /// <summary>
        /// Thrown when there is no such field in a class
        /// </summary>
        /// <param name="name">Name of the class</param>
        /// <param name="field">Field accessed</param>
        /// <param name="stack">The call stack</param>
        internal static void NoFieldError(string name, string field, string[] stack = null)
        {
            AttributeError($"'{name}' has no field called '{field}'", stack);
        }
    }
}
