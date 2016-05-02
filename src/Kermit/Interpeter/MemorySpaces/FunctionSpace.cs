using System.Collections.Generic;
using System.Linq;
using Kermit.Interpeter.Types;
using Kermit.Parser;

namespace Kermit.Interpeter.MemorySpaces
{
    /// <summary>
    /// Memory space for functions
    /// </summary>
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
