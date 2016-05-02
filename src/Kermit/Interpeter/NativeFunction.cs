using System;
using Kermit.Interpeter.Exceptions;
using Kermit.Interpeter.Types;

namespace Kermit.Interpeter
{
    /// <summary>
    /// Abstract class representing native functions
    /// </summary>
    public abstract class NativeFunction
    {
        /// <summary>
        /// The name of the function
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Throw an error
        /// </summary>
        /// <param name="msg">Message of the error</param>
        /// <param name="e">Inner exception</param>
        protected void Error(string msg, Exception e)
        {
            ThrowHelper.InterpreterException(msg, e);
        }

        /// <summary>
        /// Throw an error
        /// </summary>
        /// <param name="msg">Message of the error</param>
        protected void Error(string msg)
        {
            ThrowHelper.InterpreterException(msg);
        }

        /// <summary>
        /// Safely execute a function
        /// </summary>
        /// <param name="info">Callback information for the function</param>
        internal void SafeExecute(FunctionCallbackInfo info)
        {
            try
            {
                Execute(info);
            }
            catch (InterpreterException)
            {
                throw;
            }
            catch (Exception e)
            {
                ThrowHelper.NativeFunctionError(Name, e);
            }
        }

        /// <summary>
        /// Cast a variable o another type
        /// </summary>
        /// <typeparam name="T">Type of the cast</typeparam>
        /// <param name="var">Input value</param>
        /// <returns>The object casteds</returns>
        protected T Cast<T>(KLocal var)
        {
            return (T) var.Value.Value;
        }

        /// <summary>
        /// Tries to cast the value
        /// </summary>
        /// <typeparam name="T">Type of the cast</typeparam>
        /// <param name="var">Input value</param>
        /// <param name="casted">Outputs the value casted</param>
        /// <returns>true or false if it can be casted</returns>
        protected bool TryCast<T>(KLocal var, out T casted)
        {
            if (var.Value.Value is T)
            {
                casted = (T) var.Value.Value;
                return true;
            }
            casted = default(T);
            return false;

        }

        /// <summary>
        /// Function extended that contains the real logic for the function
        /// </summary>
        /// <param name="info">Callback information for the function</param>
        public abstract void Execute(FunctionCallbackInfo info);
    }
}
