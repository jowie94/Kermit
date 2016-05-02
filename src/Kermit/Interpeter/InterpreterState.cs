using System;
using System.Collections.Generic;
using System.Linq;
using Kermit.Interpeter.MemorySpaces;
using Kermit.Interpeter.Types;
using Kermit.Parser;

namespace Kermit.Interpeter
{
    /// <summary>
    /// Represents the state of the interpreter
    /// </summary>
    public abstract class InterpreterState
    {
        private IScope _globalScope;
        private IInterpreterListener _listener;

        internal abstract Stack<FunctionSpace> Stack { get; }
        protected readonly MemorySpace Globals = new MemorySpace("globals");

        /// <summary>
        /// The global scope of the engine
        /// </summary>
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

        /// <summary>
        /// The call stack of the interpreter
        /// </summary>
        public string[] StackTrace =>
            Stack.Select(
                x => (x.FunctionDefinition is NativeFunctionSymbol ? "(Native) " : "") + x.FunctionDefinition.Name)
                .Concat(new[] {"Global"})
                .Reverse()
                .ToArray();

        /// <summary>
        /// Get a global variable
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <returns>The value of the variable or null if not found</returns>
        public KGlobal GetGlobalVariable(string name)
        {
            return Globals.Contains(name) ? new KGlobal(name, Globals) : null;
        }

        /// <summary>
        /// Get a function
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <returns>The function or null if not found</returns>
        public KFunction GetFunction(string name)
        {
            FunctionSymbol fs = _globalScope.Resolve(name) as FunctionSymbol;
            return fs == null ? null : new KFunction(fs);
        }

        /// <summary>
        /// Call a function
        /// </summary>
        /// <param name="function">The function to be called</param>
        /// <param name="parameters">The parameters to be passed to the function</param>
        /// <returns>The result of the call</returns>
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

        /// <summary>
        /// Input/output functions of the interpreter
        /// </summary>
        public IInterpreterListener IO => _listener;
    }
}
