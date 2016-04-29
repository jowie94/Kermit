﻿using System;
using Kermit.Interpeter.Exceptions;
using Kermit.Interpeter.Types;

namespace Kermit.Interpeter
{
    public abstract class NativeFunction
    {
        public string Name { get; internal set; }

        protected void Error(string msg, Exception e)
        {
            ThrowHelper.InterpreterException(msg, e);
        }

        protected void Error(string msg)
        {
            ThrowHelper.InterpreterException(msg);
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
                ThrowHelper.NativeFunctionError(Name, e);
            }
        }

        protected T Cast<T>(KLocal var)
        {
            return (T) var.Value.Value;
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
