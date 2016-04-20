using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser;

namespace Interpeter
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
