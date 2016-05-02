using Kermit.Parser;

namespace Kermit.Interpeter
{
    /// <summary>
    /// Symbol for native functions
    /// </summary>
    internal class NativeFunctionSymbol : FunctionSymbol
    {
        public readonly NativeFunction NativeFunction;

        public NativeFunctionSymbol(string name, IScope parentScope, NativeFunction nativeFunction) : base(name, parentScope)
        {
            NativeFunction = nativeFunction;
        }
    }
}
