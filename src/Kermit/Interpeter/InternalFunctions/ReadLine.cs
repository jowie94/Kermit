using System;
using Interpeter.Types;

namespace Interpeter.InternalFunctions
{
    class ReadLine : KFunction
    {
        public override void Execute(FunctionCallbackInfo info)
        {
            Console.Write("Input: ");
            string res = Console.ReadLine();
            info.ReturnValue.Value = TypeHelper.ToKObject(res);
        }
    }
}
