﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    internal abstract class ScopedSymbol : Symbol, IScope
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
            if (ParentScope != null)
                return ParentScope.Resolve(name);
            return null;
        }

        public abstract IDictionary<string, Symbol> GetMembers();
    }
}