using System;
using System.Collections.Generic;
using Antlr.Runtime;
using Kermit.Parser.Exceptions;

namespace Kermit.Parser
{
    public partial class KermitParser : Antlr.Runtime.Parser
    {
        public bool StopOnError = true;
        public readonly IList<Exception> ErrorList = new List<Exception>();

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
            {
                PartialStatement error = new PartialStatement();
                ErrorList.Add(error);
                if (StopOnError)
                    throw error;
                return;
            }
            base.ReportError(e);
            ParserException syntaxError = ThrowHelper.SyntaxError(SourceName, e.Line, e.CharPositionInLine,
                    CreateInputError(e.Line, e.CharPositionInLine), GetErrorMessage(e, tokenNames), e);
            ErrorList.Add(syntaxError);
            if (StopOnError)
                throw syntaxError;
        }
    }
}
