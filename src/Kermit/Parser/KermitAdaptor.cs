using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace Kermit.Parser
{
    /// <summary>
    /// Custom TreeAdaptor for the parser
    /// </summary>
    public class KermitAdaptor : CommonTreeAdaptor
    {
        public override object Create(IToken payload)
        {
            return new KermitAST(payload);
        }

        public override object DupNode(object treeNode)
        {
            return treeNode == null ? null : Create(((KermitAST)treeNode).Token);
        }

        public override object ErrorNode(ITokenStream input, IToken start, IToken stop, RecognitionException e)
        {
            return new KermitErrorNode(input, start, stop, e);
        }
    }
}
