using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;

namespace Parser
{
    public class KermitErrorNode : KermitAST
    {
        public KermitErrorNode(ITokenStream input, IToken start, IToken stop, RecognitionException e) : base(start)
        {
        }
    }
}
