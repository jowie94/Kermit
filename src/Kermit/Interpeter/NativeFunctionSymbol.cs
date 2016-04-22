﻿using Kermit.Parser;

namespace Kermit.Interpeter
{
    internal class NativeFunctionSymbol : FunctionSymbol
    {
        public NativeFunction NativeFunction;

        public NativeFunctionSymbol(string name, IScope parentScope, NativeFunction nativeFunction) : base(name, parentScope)
        {
            NativeFunction = nativeFunction;
        }
    }
}
