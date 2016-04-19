using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpeter.InternalFunctions
{
    class PrintStackTrace : KFunction
    {
        public override void Execute(FunctionCallbackInfo info)
        {
            string[] stack = info.InterpreterState.StackTrace;
            Console.WriteLine(string.Join("\n", stack));
        }
    }
}
