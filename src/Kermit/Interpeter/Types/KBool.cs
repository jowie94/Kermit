using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpeter.Types
{
    public class KBool : KObject
    {
        public new bool Value;

        public KBool(bool value)
        {
            Value = value;
        }

        protected bool Equals(KBool other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((KBool) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static implicit operator bool(KBool b)
        {
            return b.Value;
        }

        public static implicit operator KBool(bool b)
        {
            return new KBool(b);
        }

        public static implicit operator KBool(KString str)
        {
            return TypeHelper.ToBool(str);
        }

        public static implicit operator KBool(KNumber num)
        {
            return TypeHelper.ToBool(num);
        }

        public static implicit operator KBool(KChar ch)
        {
            return TypeHelper.ToBool(ch);
        }

        public static bool operator !(KBool element)
        {
            return !element.Value;
        }

        public static KBool True => new KBool(true);
        public static KBool False => new KBool(false);

        protected override bool Not()
        {
            return !this;
        }
    }
}
