using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interpeter.MemorySpaces;
using Interpeter.Types;
using Parser;

namespace Interpeter
{
    public abstract class InterpreterState
    {
        public MemorySpace _globals = new MemorySpace("globals"); // TODO: Public just for debugging! MUST BE INTERNAL
        private IScope _globalScope;
        internal Stack<FunctionSpace> _stack = new Stack<FunctionSpace>();

        public IScope GlobalScope
        {
            get { return _globalScope; }
            protected set
            {
                if (value == null)
                    throw new ArgumentNullException();
                _globalScope = value;
            }
        }

        public string[] StackTrace =>
            _stack.Select(x => (x.FunctionDefinition is NativeFunctionSymbol ? "(Native) " : "") + x.FunctionDefinition.Name).Reverse().ToArray();

        public KGlobal GetGlobalVariable(string name)
        {
            return _globals.Contains(name) ? new KGlobal(name, _globals) : null;
        }

        public KFunction GetFunction(string name)
        {
            FunctionSymbol fs = _globalScope.Resolve(name) as FunctionSymbol;
            return fs == null ? null : new KFunction(fs);
        }

        public abstract KObject CallFunction(KFunction function, List<KLocal> parameters);
    }
}
