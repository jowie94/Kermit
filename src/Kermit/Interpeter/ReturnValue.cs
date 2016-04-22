using System;
using Kermit.Interpeter.Types;

namespace Kermit.Interpeter
{
    public class ReturnValue : Exception
    {
        public KObject Value;
    }
}