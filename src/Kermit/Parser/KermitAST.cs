using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace Kermit.Parser
{
    /// <summary>
    /// Basic KermitAST tree node
    /// </summary>
    public class KermitAST : CommonTree
    {
        public IScope Scope;

        public KermitAST(IToken t) : base(t) {}
    }
}
