using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interpeter.MemorySpaces;
using Interpeter.Types;

namespace Interpeter
{
    public class InterpreterState
    {
        public MemorySpace _globals = new MemorySpace("globals"); // TODO: Public just for debugging! MUST BE INTERNAL
        internal Stack<FunctionSpace> _stack = new Stack<FunctionSpace>();

        public string[] StackTrace =>
            _stack.Select(x => (x.FunctionDefinition is NativeFunctionSymbol ? "(Native) " : "") + x.FunctionDefinition.Name).Reverse().ToArray();

        public KGlobal GetGlobalVariable(string name)
        {
            return _globals.Contains(name) ? new KGlobal(name, _globals) : null;
        }
    }
}
