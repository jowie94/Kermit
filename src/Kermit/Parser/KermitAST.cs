﻿using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace Kermit.Parser
{
    public class KermitAST : CommonTree
    {
        public IScope Scope;

        public KermitAST(IToken t) : base(t) {}
    }
}
