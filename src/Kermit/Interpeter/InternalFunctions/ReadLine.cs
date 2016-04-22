using System;
using Kermit.Interpeter.Types;

namespace Kermit.Interpeter.InternalFunctions
{
    class ReadLine : NativeFunction
    {
        public override void Execute(FunctionCallbackInfo info)
        {
            Console.Write("Input: ");
            string res = Console.ReadLine();
            info.ReturnValue.Value = TypeHelper.ToKObject(res);
        }
    }
}
