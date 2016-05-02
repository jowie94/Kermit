using System;
using Kermit.Interpeter.Types;

namespace Kermit.Interpeter
{
    /// <summary>
    /// Class holding return values.
    /// It is an exception as it is used to break the execution flow when a function returns.
    /// </summary>
    public class ReturnValue : Exception
    {
        private KObject _value;

        public KObject Value
        {
            get { return _value; }
            set {
                _value = value ?? new KVoid();
            }
        }
    }
}
