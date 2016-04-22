using Kermit.Parser;

namespace Kermit.Interpeter.Types
{
    public class KFunction : KObject
    {
        internal new readonly FunctionSymbol Value;

        public bool IsNative { get; }

        public string Name => Value.Name;

        internal KFunction(FunctionSymbol fSymbol)
        {
            Value = fSymbol;
            IsNative = Value is NativeFunctionSymbol;
        }

        protected override bool Not()
        {
            return false;
        }
    }
}
