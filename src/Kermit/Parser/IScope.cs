using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public interface IScope
    {
        /// <summary>
        /// The name of the scope
        /// </summary>
        string ScopeName { get; }

        Symbol[] SymbolList { get; }

        /// <summary>
        /// The parent scope. Here we will look for more symbols.
        /// </summary>
        IScope EnclosingScope { get; }

        /// <summary>
        /// Define a new symbol in the current scope.
        /// </summary>
        /// <param name="sym">The new symbol to be defined</param>
        void Define(Symbol sym);

        /// <summary>
        /// Look up name in this scope or in the encolsing if not found
        /// </summary>
        /// <param name="name">Name to look for.</param>
        /// <returns>The found symbol or null if not found.</returns>
        Symbol Resolve(string name);
    }
}
