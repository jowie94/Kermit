using System;
using Antlr.Runtime;

namespace Parser
{
    public partial class KermitParser : Antlr.Runtime.Parser
    {
        public override void ReportError(RecognitionException e)
        {
            base.ReportError(e);
            Console.WriteLine("Error in parser at line " + e.Line + ":" + e.CharPositionInLine);
            Console.WriteLine(e.Message);
        }
    }
}
