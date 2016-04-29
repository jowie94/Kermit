using System.Collections.Generic;
using System.Linq;

namespace Kermit.Parser
{
    public abstract class BaseScope : IScope
    {
        public string ScopeName { get; protected set; }
        public IScope EnclosingScope { get; }

        public virtual Symbol[] SymbolList => _symbols.Values.ToArray();

        readonly IDictionary<string, Symbol> _symbols = new Dictionary<string, Symbol>();
        

        protected BaseScope(IScope parent)
        {
            EnclosingScope = parent;
        }

        public virtual void Define(Symbol sym)
        {
            _symbols[sym.Name] = sym;
            sym.Scope = this;
        }

        public virtual Symbol Resolve(string name)
        {
            Symbol s;
            if (_symbols.TryGetValue(name, out s)) return s;
            return ParentScope?.Resolve(name);
        }

        public IScope ParentScope => EnclosingScope;

        public override string ToString()
        {
            string s = "";
            if (EnclosingScope != null)
                s = EnclosingScope + "\n";
            return s + ScopeName + " " + string.Join(";", _symbols.Select(x => x.Key + "=" + x.Value.ToString()));
        }
    }
}
