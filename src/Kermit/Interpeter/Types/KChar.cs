using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpeter.Types
{
    class KChar : KElement
    {
        public new char Value;

        public KChar(char value)
        {
            Value = value;
            Type = KType.Char;
        }

        protected bool Equals(KChar other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((KChar) obj);
        }
    }
}
