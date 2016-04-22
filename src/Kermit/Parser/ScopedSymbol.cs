using System.Collections.Generic;
using System.Linq;

namespace Kermit.Parser
{
    public abstract class ScopedSymbol : Symbol, IScope
    {
        public IScope EnclosingScope { get; private set; }

        public IScope ParentScope
        {
            get { return EnclosingScope; }
        }

        public string ScopeName
        {
            get { return Name; }
        }

        public Symbol[] SymbolList => EnclosingScope.SymbolList.Concat(GetMembers().Values).ToArray();

        internal ScopedSymbol(string name, IScope enclosingScope) : base(name)
        {
            EnclosingScope = enclosingScope;
        }

        public void Define(Symbol sym)
        {
            // TODO: FIXME Return nicer exception
            GetMembers().Add(sym.Name, sym);
            sym.Scope = this;
        }

        public Symbol Resolve(string name)
        {
            Symbol s;
            if (GetMembers().TryGetValue(name, out s))
                return s;
            if (ParentScope != null)
                return ParentScope.Resolve(name);
            return null;
        }

        public abstract IDictionary<string, Symbol> GetMembers();
    }
}
