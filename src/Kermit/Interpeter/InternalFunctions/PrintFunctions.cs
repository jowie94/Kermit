using System.Linq;
using Kermit.Parser;

namespace Kermit.Interpeter.InternalFunctions
{
    class PrintFunctions : NativeFunction
    {
        public override void Execute(FunctionCallbackInfo info)
        {
            string msg = string.Join("\n", info.InterpreterState.GlobalScope.SymbolList.Where(x => x is FunctionSymbol)
                .Select(x => "- " + x.Name).ToArray());
            info.InterpreterState.IO.Write(msg);
        }
    }
}
