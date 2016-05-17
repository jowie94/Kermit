using System;

namespace Kermit.Interpeter.Types
{
    /// <summary>
    /// Type representing a boolean
    /// </summary>
    public class KBool : KObject
    {
        public new bool Value;

        public KBool(bool value)
        {
            Value = value;
        }

        private bool Equals(KBool other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(TypeHelper.Cast<KBool>(obj));
        }

        public override int GetHashCode() => base.GetHashCode();

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

        protected override object GetValue()
        {
            return Value;
        }

        protected override void SetValue(object obj)
        {
            if (obj is bool)
                Value = (bool) obj;
            else
                throw new InvalidCastException("The value is not of type bool");
        }


        protected override bool Not()
        {
            return !this;
        }
    }
}
