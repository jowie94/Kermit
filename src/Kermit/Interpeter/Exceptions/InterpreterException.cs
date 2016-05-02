using System;

namespace Kermit.Interpeter.Exceptions
{
    /// <summary>
    /// Simbolizes an internal exception of the interpreter
    /// </summary>
    [Serializable]
    public class InterpreterException : Exception
    {
        /// <summary>
        /// The Stack of function calls
        /// </summary>
        public string[] CallStack { get; internal set; }

        public InterpreterException(string message, string[] stack = null) : base(message)
        {
            CallStack = stack;
        }

        public InterpreterException(string message, Exception innerException, string[] stack = null)
            : base(message, innerException)
        {
            CallStack = stack;
        }

        /// <summary>
        /// Format the call stack as an string separated by \n
        /// </summary>
        /// <returns></returns>
        public string GetCallStack()
        {
            string res = "";
            if (CallStack != null)
                res = string.Join("\n", CallStack);
            return res;
        }
    }
}
