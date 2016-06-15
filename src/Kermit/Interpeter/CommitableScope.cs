using System.Collections.Generic;
using System.Linq;
using Kermit.Parser;

namespace Kermit.Interpeter
{
    /// <summary>
    /// Represents a comitable scope that can be commited or reverted
    /// </summary>
    internal abstract class CommitableScope : BaseScope
    {
        private readonly IDictionary<string, Symbol> _tmpSymbols = new Dictionary<string, Symbol>();

        public override Symbol[] SymbolList => base.SymbolList.Concat(_tmpSymbols.Values).ToArray();

        protected CommitableScope(IScope parent) : base(parent) {}

        public void CommitScope()
        {
            _tmpSymbols.ToList().ForEach(x => base.Define(x.Value));
            _tmpSymbols.Clear();
        }

        public void RevertScope()
        {
            _tmpSymbols.Clear();
        }

        public override void Define(Symbol sym)
        {
            _tmpSymbols[sym.Name] = sym;
            sym.Scope = this;
        }

        public override Symbol Resolve(string name)
        {
            Symbol s = base.Resolve(name);
            if (s == null && _tmpSymbols.TryGetValue(name, out s)) return s;
            return s;
        }
    }
}
