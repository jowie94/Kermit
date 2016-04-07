using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpeter.Types
{
    class KString : KElement
    {
        public new string Value;

        public KString(string value)
        {
            Value = value;
            Type = KType.String;
        }

        protected bool Equals(KString other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((KString) obj);
        }

        public static explicit operator KString(string str)
        {
            return new KString(str);
        }
    }
}
