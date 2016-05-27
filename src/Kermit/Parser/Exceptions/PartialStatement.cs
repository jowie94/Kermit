using System;

namespace Kermit.Parser.Exceptions
{
    public class PartialStatement : Exception
    {
        public PartialStatement(ParserException parserException) : base("Partial statement", parserException) {}
    }
}
