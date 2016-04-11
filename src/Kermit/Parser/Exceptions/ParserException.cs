using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Exceptions
{
    public class ParserException : Exception
    {
        public ParserException(string msg, Exception inner) : base(msg, inner) {}
    }
}
