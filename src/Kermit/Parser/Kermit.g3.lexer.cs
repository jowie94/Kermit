using System;
using Antlr.Runtime;

namespace Kermit.Parser
{
    public partial class KermitLexer
    {
        private string CreateInputError(int line, int pos)
        {
            string[] str = input.ToString().Split('\n');
            string bot = string.Empty;
            if (pos != 0)
                bot = $"\n{new string(' ', pos - 1)} ^";
            line = line == 0 ? 0 : line - 1;
            return $"{str[line]}{bot}";
        }


        public override void ReportError(RecognitionException e)
        {
            base.ReportError(e);
            Console.WriteLine("Error in lexer at line " + e.Line + ":" + e.CharPositionInLine);
            throw ThrowHelper.SyntaxError(SourceName, e.Line, e.CharPositionInLine,
                CreateInputError(e.Line, e.CharPositionInLine), "Invalid syntax", e);
        }
    }
}

