using System;
using System.ComponentModel;
using Antlr.Runtime;
using Parser.Exceptions;

namespace Parser
{
    public partial class KermitParser : Antlr.Runtime.Parser
    {
        public override void ReportError(RecognitionException e)
        {
            if (e.Token.Type == EOF)
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
            string msg = $"File: {SourceName} at line {e.Line}:{e.CharPositionInLine}\nSyntaxError: {GetErrorMessage(e, tokenNames)}";
            throw new ParserException(msg, e);
        }
    }
}
