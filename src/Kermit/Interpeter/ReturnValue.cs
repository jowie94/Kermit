using System;
using Interpeter.Types;

namespace Interpeter
{
    public class ReturnValue : Exception
    {
        public KObject Value;
    }
}