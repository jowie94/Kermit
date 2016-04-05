using System;
using Antlr.Runtime;

namespace Parser
{
    public partial class KermitParser : Antlr.Runtime.Parser
    {
        public override void ReportError(RecognitionException e)
        {
            if (e.Token.Type == EOF)
                throw new PartialStatement();
            base.ReportError(e);
            Console.WriteLine(e.GetType());
            Console.WriteLine(e.Token.Type);
            Console.WriteLine("Error in parser at line " + e.Line + ":" + e.CharPositionInLine);
            Console.WriteLine(e.Message);
        }
    }
}
