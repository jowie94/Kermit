using System.Linq;
using Kermit.Interpeter.Types;
using Kermit.Parser;

namespace Kermit.Interpeter.InternalFunctions
{
    class PrintFunctions : NativeFunction
    {
        public override void Execute(FunctionCallbackInfo info)
        {
            string msg = string.Join("\n", info.InterpreterState.GlobalScope.SymbolList.Where(x => x is FunctionSymbol)
                .Select(x => "- " + x.Name).ToArray());
            KFunction write = info.InterpreterState.GetFunction("Write");
            info.InterpreterState.CallFunction(write, TypeHelper.ToParameterList(msg));
        }
    }
}
