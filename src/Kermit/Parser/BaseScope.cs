using System.Collections.Generic;
using System.Linq;

namespace Kermit.Parser
{
    /// <summary>
    /// Basic scope class defining basic functions
    /// </summary>
    public abstract class BaseScope : IScope
    {
        /// <summary>
        /// The name of the scope
        /// </summary>
        public string ScopeName { get; protected set; }
        /// <summary>
        /// The parent scope
        /// </summary>
        public IScope EnclosingScope { get; }
        /// <summary>
        /// The symbol list inside the array
        /// </summary>
        public virtual Symbol[] SymbolList => _symbols.Values.ToArray();
        /// <summary>
        /// Internal symbol table
        /// </summary>
        readonly IDictionary<string, Symbol> _symbols = new Dictionary<string, Symbol>();
        

        protected BaseScope(IScope parent)
        {
            EnclosingScope = parent;
        }

        /// <summary>
        /// Define a new symbol in the table
        /// </summary>
        /// <param name="sym">Symbol to be added</param>
        public virtual void Define(Symbol sym)
        {
            _symbols[sym.Name] = sym;
            sym.Scope = this;
        }

        /// <summary>
        /// Resolve a name inside the table
        /// </summary>
        /// <param name="name">The name to be resolved</param>
        /// <returns>A symbol with that name or null if not found</returns>
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
