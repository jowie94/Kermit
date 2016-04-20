using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interpeter.Types;

namespace Interpeter.InternalFunctions
{
    class PrintStackTrace : NativeFunction
    {
        public override void Execute(FunctionCallbackInfo info)
        {
            string[] stack = info.InterpreterState.StackTrace;
            KFunction write = info.InterpreterState.GetFunction("Write");
            info.InterpreterState.CallFunction(write, TypeHelper.ToParameterList(string.Join("\n", stack)));
        }
    }
}
