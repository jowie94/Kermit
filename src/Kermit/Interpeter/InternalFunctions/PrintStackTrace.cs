using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kermit.Interpeter.Types;

namespace Kermit.Interpeter.InternalFunctions
{
    class PrintStackTrace : NativeFunction
    {
        public override void Execute(FunctionCallbackInfo info)
        {
            string[] stack = info.InterpreterState.StackTrace;
            info.InterpreterState.IO.Write(string.Join("\n", stack));
        }
    }
}
