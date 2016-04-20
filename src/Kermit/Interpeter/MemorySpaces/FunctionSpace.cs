using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser;
using Interpeter.Types;

namespace Interpeter.MemorySpaces
{
    class FunctionSpace : MemorySpace
    {
        public readonly FunctionSymbol FunctionDefinition; // Function being executed

        public FunctionSpace(FunctionSymbol function) : base(function.Name + "_invocation")
        {
            FunctionDefinition = function;
        }

        public List<KLocal> GetArgumentList()
        {
            return Members.Values.ToList();
        }
    }
}
