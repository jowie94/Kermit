using System;
using Kermit.Interpeter.Types;

namespace Kermit.Interpeter.InternalFunctions
{
    /// <summary>
    /// Reads a line
    /// </summary>
    class ReadLine : NativeFunction
    {
        public override void Execute(FunctionCallbackInfo info)
        {
            Console.Write("Input: ");
            string res = info.InterpreterState.IO.ReadLine();
            info.ReturnValue.Value = TypeHelper.ToKObject(res);
        }
    }
}
