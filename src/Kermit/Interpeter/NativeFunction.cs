using System;
using Kermit.Interpeter.Types;

namespace Kermit.Interpeter
{
    public abstract class NativeFunction
    {
        public string Name { get; internal set; }

        protected void Error(string msg, Exception e)
        {
            throw new InterpreterException(msg, e);
        }

        protected void Error(string msg)
        {
            throw new InterpreterException(msg);
        }

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
                Error($"Function {Name} throwed an exception", e);
            }
        }

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

        public abstract void Execute(FunctionCallbackInfo info);
    }
}
