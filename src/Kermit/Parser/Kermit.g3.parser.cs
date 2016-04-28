using Antlr.Runtime;
using Kermit.Parser.Exceptions;

namespace Kermit.Parser
{
    public partial class KermitParser : Antlr.Runtime.Parser
    {
        private string CreateInputError(int line, int pos)
        {
            string[] str = input.ToString().Split('\n');
            pos = pos == 0 ? 0 : pos - 1;
            return $"{str[line - 1]}\n{new string(' ', pos)} ^";
        }

        public override void ReportError(RecognitionException e)
        {
            if (e.Token != null && e.Token.Type == EOF)
            {
                if (currentScope.EnclosingScope != null)
                {
                    currentScope = currentScope.EnclosingScope;
                    currentScope = currentScope.EnclosingScope;
                }
                throw new PartialStatement();
            }
            base.ReportError(e);
            //Console.WriteLine(e.GetType());
            //Console.WriteLine(e.Token.Type);
            //Console.WriteLine(e.Message);
            //Console.WriteLine($"Error in parser at line {e.Line}: {e.CharPositionInLine}\n{GetErrorMessage(e, tokenNames)}");
            string msg =
                $"File: {SourceName} at line {e.Line}:{e.CharPositionInLine}\n{CreateInputError(e.Line, e.CharPositionInLine)}\nSyntaxError: {GetErrorMessage(e, tokenNames)}";
            throw new ParserException(msg, e);
        }
    }
}
