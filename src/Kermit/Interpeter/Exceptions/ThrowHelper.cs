using System;

namespace Kermit.Interpeter.Exceptions
{
    internal static class ThrowHelper
    {
        internal static void InterpreterException(string message, string[] stack = null)
        {
            throw new InterpreterException(message, stack);
        }

        internal static void InterpreterException(string message, Exception inner, string[] stack = null)
        {
            throw new InterpreterException(message, inner, stack);
        }

        internal static void NameNotExists(string name, string[] stack = null)
        {
            NameError("name '" + name + "' is not defined", stack);
        }

        internal static void NameError(string message, string[] stack = null)
        {
            InterpreterException("NameError: " + message, stack);
        }

        internal static void TypeError(string message, string[] stack = null)
        {
            InterpreterException("TypeError: " + message, stack);
        }

        internal static void NativeFunctionError(string name, Exception e)
        {
            InterpreterException($"NativeError: {e.GetType().Name} thrown by {name}:\nMessage: {e.Message}");
        }

        internal static void AttributeError(string message, string[] stack = null)
        {
            InterpreterException("AttributeError: " + message, stack);
        }

        internal static void NoFieldError(string name, string field, string[] stack = null)
        {
            AttributeError($"'{name}' has no field called '{field}'", stack);
        }
    }
}
