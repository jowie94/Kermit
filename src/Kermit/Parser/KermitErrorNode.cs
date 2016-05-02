using Antlr.Runtime;

namespace Kermit.Parser
{
    /// <summary>
    /// Kermit AST error node
    /// </summary>
    public class KermitErrorNode : KermitAST
    {
        public KermitErrorNode(ITokenStream input, IToken start, IToken stop, RecognitionException e) : base(start)
        {
        }
    }
}
