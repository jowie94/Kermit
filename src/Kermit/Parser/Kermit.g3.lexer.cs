using System;
using Antlr.Runtime;

namespace Parser
{
    public partial class KermitLexer
    {
        public override void ReportError(RecognitionException e)
        {
            base.ReportError(e);
            Console.WriteLine("Error in lexer at line " + e.Line + ":" + e.CharPositionInLine);
            throw e;
        }
    }
}

