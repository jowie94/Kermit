using System.Collections.Generic;
using System.Linq;

namespace Kermit.Parser
{
    /// <summary>
    /// Symbol containing a scope (eg. functions, classes, structs...)
    /// </summary>
    public abstract class ScopedSymbol : Symbol, IScope
    {
        /// <summary>
        /// Parent scope
        /// </summary>
        public IScope EnclosingScope { get; }
        
        /// <summary>
        /// Parent scope
        /// </summary>
        public IScope ParentScope => EnclosingScope;
        
        /// <summary>
        /// Scope name
        /// </summary>
        public string ScopeName => Name;
        
        /// <summary>
        /// List of symbols
        /// </summary>
        public Symbol[] SymbolList => EnclosingScope.SymbolList.Concat(GetMembers().Values).ToArray();

        internal ScopedSymbol(string name, IScope enclosingScope) : base(name)
        {
            EnclosingScope = enclosingScope;
        }

        public void Define(Symbol sym)
        {
            GetMembers().Add(sym.Name, sym);
            sym.Scope = this;
        }

        /// <summary>
        /// Look up name in this scope or in the parent if not found
        /// </summary>
        /// <param name="name">Name to look for</param>
        /// <returns>The found symbol or null if not found.</returns>
        public Symbol Resolve(string name)
        {
            Symbol s;
            if (GetMembers().TryGetValue(name, out s))
                return s;
            return ParentScope?.Resolve(name);
        }

        public abstract IDictionary<string, Symbol> GetMembers();
    }
}
