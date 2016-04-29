using System;
using System.Collections.Generic;
using System.Linq;
using Kermit.Interpeter.MemorySpaces;
using Kermit.Interpeter.Types;
using Kermit.Parser;

namespace Kermit.Interpeter
{
    public abstract class InterpreterState
    {
        private IScope _globalScope;
        private IInterpreterListener _listener;

        internal abstract Stack<FunctionSpace> Stack { get; }
        protected readonly MemorySpace Globals = new MemorySpace("globals");

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
            Stack.Select(
                x => (x.FunctionDefinition is NativeFunctionSymbol ? "(Native) " : "") + x.FunctionDefinition.Name)
                .Reverse()
                .ToArray();

        public KGlobal GetGlobalVariable(string name)
        {
            return Globals.Contains(name) ? new KGlobal(name, Globals) : null;
        }

        public KFunction GetFunction(string name)
        {
            FunctionSymbol fs = _globalScope.Resolve(name) as FunctionSymbol;
            return fs == null ? null : new KFunction(fs);
        }

        public abstract KObject CallFunction(KFunction function, List<KLocal> parameters);

        protected IInterpreterListener Listener
        {
            get { return _listener; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                _listener = value;
            }
        }

        public IInterpreterListener IO => _listener;
    }
}
