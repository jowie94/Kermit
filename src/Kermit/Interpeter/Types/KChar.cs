using System;
using Kermit.Interpeter.Exceptions;

namespace Kermit.Interpeter.Types
{
    public class KChar : KObject, IComparable
    {
        public new char Value;

        public KChar(char value)
        {
            Value = value;
        }

        private bool Equals(KChar other)
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

        public override int GetHashCode() => base.GetHashCode();

        public int CompareTo(object obj)
        {
            KChar ch = obj as KChar;
            if (ch != null)
                return Value.CompareTo(ch.Value);
            ThrowHelper.TypeError("Object is not of type KChar");
            return 0;
        }

        public static implicit operator KChar(char ch)
        {
            return new KChar(ch);
        }

        public static implicit operator char(KChar ch)
        {
            return ch.Value;
        }

        protected override object GetValue()
        {
            return Value;
        }

        protected override void SetValue(object obj)
        {
            if (obj is char)
                Value = (char) obj;
            else
                throw new InvalidCastException("The value is not of type char");
        }

        protected override bool Not()
        {
            return !(KBool) this;
        }
    }
}
