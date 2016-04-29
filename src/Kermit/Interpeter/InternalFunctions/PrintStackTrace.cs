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
