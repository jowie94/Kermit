using System;
using Antlr.Runtime;
using Kermit.Parser.Exceptions;

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
            ParserException syntaxError = ThrowHelper.SyntaxError(SourceName, e.Line, e.CharPositionInLine,
                input.ToString(), "Invalid syntax", e);
            if (e.UnexpectedType == EOF)
                throw new PartialStatement(syntaxError);
            base.ReportError(e);
            //Console.WriteLine("Error in lexer at line " + e.Line + ":" + e.CharPositionInLine);
            throw syntaxError;
        }
    }
}

