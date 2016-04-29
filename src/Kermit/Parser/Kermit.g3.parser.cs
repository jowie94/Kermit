using Antlr.Runtime;
using Kermit.Parser.Exceptions;

namespace Kermit.Parser
{
    public partial class KermitParser : Antlr.Runtime.Parser
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
            if (currentScope.EnclosingScope != null)
            {
                currentScope = currentScope.EnclosingScope;
                currentScope = currentScope.EnclosingScope;
            }
            if (e.Token != null && e.Token.Type == EOF)
                throw new PartialStatement();
            base.ReportError(e);
            ThrowHelper.SyntaxError(SourceName, e.Line, e.CharPositionInLine,
                CreateInputError(e.Line, e.CharPositionInLine), GetErrorMessage(e, tokenNames), e);
        }
    }
}
