using Antlr.Runtime;

namespace Kermit.Parser
{
    public class KermitErrorNode : KermitAST
    {
        public KermitErrorNode(ITokenStream input, IToken start, IToken stop, RecognitionException e) : base(start)
        {
        }
    }
}
