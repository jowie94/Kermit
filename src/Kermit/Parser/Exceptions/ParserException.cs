using System;

namespace Kermit.Parser.Exceptions
{
    public class ParserException : Exception
    {
        public ParserException(string msg, Exception inner) : base(msg, inner) {}
    }
}
