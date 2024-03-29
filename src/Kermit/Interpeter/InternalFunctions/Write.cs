﻿using Kermit.Interpeter.Types;

namespace Kermit.Interpeter.InternalFunctions
{
    /// <summary>
    /// Writes in the console
    /// </summary>
    internal class Write : NativeFunction
    {
        public override void Execute(FunctionCallbackInfo info)
        {
            string msg = "";
            if (info.Length > 0)
                msg = TypeHelper.ToString(info[0].Value);
            if (info.Length > 1)
            {
                object[] obj = new object[info.Length - 1];
                for (int i = 1; i < info.Length; ++i)
                    obj[i - 1] = Cast<object>(info[i]);
                msg = string.Format(msg, obj);
            }
                
            info.InterpreterState.IO.Write(msg);
        }
    }
}
