using System;

namespace Kermit.Interpeter.Types
{
    public class KString : KObject, IComparable
    {
        public new string Value;

        public KString(string value)
        {
            Value = value;
        }

        private bool Equals(KString other)
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

        public override int GetHashCode() => base.GetHashCode();

        public static explicit operator KString(string str)
        {
            return new KString(str);
        }

        public static implicit operator string(KString kstr)
        {
            return kstr.Value;
        }

        public int CompareTo(object obj)
        {
            KString ch = obj as KString;
            if (ch != null)
                return string.Compare(Value, ch.Value, StringComparison.Ordinal);
            throw new ArgumentException("Object is not of type character");
        }

        protected override object GetValue()
        {
            return Value;
        }

        protected override void SetValue(object obj)
        {
            if (obj is string)
                Value = (string) obj;
            else
                throw new InvalidCastException("The value is not of type string");
        }

        protected override bool Not()
        {
            return !(KBool)this;
        }
    }
}
