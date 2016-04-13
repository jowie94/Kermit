using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser;

namespace Interpeter
{
    public class NativeFunctionSymbol : FunctionSymbol
    {
        public KFunction NativeFunction;

        public NativeFunctionSymbol(string name, IScope parentScope, KFunction nativeFunction) : base(name, parentScope)
        {
            NativeFunction = nativeFunction;
        }
    }
}
