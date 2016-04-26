using System;
using Kermit.Interpeter.Types;

namespace Kermit.Interpeter
{
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
