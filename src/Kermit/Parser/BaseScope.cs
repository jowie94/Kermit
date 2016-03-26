using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser;

namespace Terminal
{
    public abstract class BaseScope : IScope
    {
        public string ScopeName { get; protected set; }
        public IScope EnclosingScope { get; private set; }

        IDictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();

        protected BaseScope(IScope parent)
        {
            EnclosingScope = parent;
        }

        public void Define(Symbol sym)
        {
            symbols.Add(sym.Name, sym);
            sym.Scope = this;
        }

        public Symbol Resolve(string name)
        {
            Symbol s;
            if (symbols.TryGetValue(name, out s)) return s;
            return ParentScope != null ? ParentScope.Resolve(name) : null;
        }

        public IScope ParentScope
        {
            get { return EnclosingScope; }
        }

        public override string ToString()
        {
            string s = "";
            if (EnclosingScope != null)
                s = EnclosingScope.ToString() + "\n";
            return s + ScopeName + " " + string.Join(";", symbols.Select(x => x.Key + "=" + x.Value.ToString()));
        }
    }
}
