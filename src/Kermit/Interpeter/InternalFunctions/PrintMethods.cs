namespace Kermit.Interpeter.InternalFunctions
{
    class PrintMethods : NativeFunction
    {
        public override void Execute(FunctionCallbackInfo info)
        {
            for (int i = 0; i < info.Length; ++i)
                info.InterpreterState.IO.Write(string.Join(", ", info[i].Value.Methods));
        }
    }
}
