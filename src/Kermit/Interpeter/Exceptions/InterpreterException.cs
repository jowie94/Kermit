using System;
using System.Linq;

namespace Kermit.Interpeter.Exceptions
{
    [Serializable]
    public class InterpreterException : Exception
    {
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

        public string GetCallStack()
        {
            string res = "";
            if (CallStack != null)
                res = string.Join("\n", CallStack);
            return res;
        }
    }
}
