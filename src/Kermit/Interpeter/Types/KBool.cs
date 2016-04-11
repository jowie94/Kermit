using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpeter.Types
{
    public class KBool : KElement
    {
        public new bool Value;

        public KBool(bool value)
        {
            Value = value;
            Type = KType.Bool;
        }

        protected bool Equals(KBool other)
        {
            return Value == other.Value;
        }

        public static KBool ToKBool(KElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (element is KString)
                return (KString) element;
            if (element is KNumber)
                return (KNumber) element;
            if (element is KBool)
                return (KBool) element;
            return (KBool) element;
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
            return str.Value.Length > 0;
        }

        public static implicit operator KBool(KNumber num)
        {
            return num.ToInt() != 0;
        }

        public static implicit operator KBool(KChar ch)
        {
            return ch.Value != null;
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
