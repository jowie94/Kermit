using System;

namespace Kermit.Parser.Exceptions
{
    public class PartialStatement : Exception
    {
        public PartialStatement() : base("Partial statement") {}
    }
}
