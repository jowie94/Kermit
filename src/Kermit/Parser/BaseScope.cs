using System.Collections.Generic;
using System.Linq;

namespace Kermit.Parser
{
    public abstract class BaseScope : IScope
    {
        public string ScopeName { get; protected set; }
        public IScope EnclosingScope { get; private set; }

        public Symbol[] SymbolList => _symbols.Values.Concat(_tmpSymbols.Values).ToArray();

        IDictionary<string, Symbol> _symbols = new Dictionary<string, Symbol>();
        IDictionary<string, Symbol> _tmpSymbols = new Dictionary<string, Symbol>();

        protected BaseScope(IScope parent)
        {
            EnclosingScope = parent;
        }

        public void CommitScope()
        {
            _tmpSymbols.ToList().ForEach(x => _symbols.Add(x));
            _tmpSymbols.Clear();
        }

        public void RevertScope()
        {
            _tmpSymbols.Clear();
        }

        public void Define(Symbol sym)
        {
            _tmpSymbols.Add(sym.Name, sym);
            sym.Scope = this;
        }

        public Symbol Resolve(string name)
        {
            Symbol s;
            if (_symbols.TryGetValue(name, out s)) return s;
            if (_tmpSymbols.TryGetValue(name, out s)) return s;
            return ParentScope != null ? ParentScope.Resolve(name) : null;
        }

        public IScope ParentScope => EnclosingScope;

        public override string ToString()
        {
            string s = "";
            if (EnclosingScope != null)
                s = EnclosingScope.ToString() + "\n";
            return s + ScopeName + " " + string.Join(";", _symbols.Select(x => x.Key + "=" + x.Value.ToString()));
        }
    }
}
