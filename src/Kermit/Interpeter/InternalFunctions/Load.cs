using System;
using Kermit.Interpeter.Exceptions;
using Kermit.Interpeter.Types;
using Kermit.Parser.Exceptions;

namespace Kermit.Interpeter.InternalFunctions
{
    class Load : NativeFunction
    {
        public override void Execute(FunctionCallbackInfo info)
        {
            if (info.Length != 1)
                Error("Bad number of arguments");
            if (!info[0].Value.IsString)
                throw new ArgumentException("Argument must be a string");
            try
            {
                info.InterpreterState.LoadScript(info[0].Value.Cast<KString>());
            }
            catch (ParserException e)
            {
                throw new InterpreterException("Invalid input\n" + e.Message);
            }
        }
    }
}
