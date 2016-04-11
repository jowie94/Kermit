using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpeter.Types
{
    public class KChar : KElement, IComparable
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

        public int CompareTo(object obj)
        {
            KChar ch = obj as KChar;
            if (ch != null)
                return Value.CompareTo(ch.Value);
            throw new ArgumentException("Object is not of type character");
        }

        public static implicit operator KChar(char ch)
        {
            return new KChar(ch);
        }

        public static implicit operator char(KChar ch)
        {
            return ch.Value;
        }

        protected override bool Not()
        {
            return !(KBool) this;
        }
    }
}
