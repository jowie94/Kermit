﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    internal class FunctionSymbol : ScopedSymbol
    {
        IDictionary<string, Symbol> formalArgs = new Dictionary<string, Symbol>();
        internal KermitAST BlockAST;

        public new string Name
        {
            get { return base.Name + '(' + formalArgs.Keys.ToString() + ')'; }
        }

        public FunctionSymbol(string name, IScope parentScope) : base(name, parentScope) {}

        public override IDictionary<string, Symbol> GetMembers()
        {
            return formalArgs;
        }
    }
}