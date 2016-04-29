using System.Collections.Generic;
using System.Linq;

namespace Kermit.Parser
{
    public abstract class ScopedSymbol : Symbol, IScope
    {
        public IScope EnclosingScope { get; }

        public IScope ParentScope => EnclosingScope;

        public string ScopeName => Name;

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
