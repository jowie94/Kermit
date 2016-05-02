using System;

namespace Kermit.Interpeter.Types
{
    /// <summary>
    /// Type representing a type
    /// </summary>
    class KType : KObject
    {
        public new Type Value;

        public KType(Type value)
        {
            Value = value;
        }

        private bool Equals(KType other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((KType) obj);
        }

        public override int GetHashCode() => base.GetHashCode();

        protected override object GetValue()
        {
            return Value;
        }

        protected override void SetValue(object obj)
        {
            if (obj is Type)
                Value = (Type) obj;
            else
                throw new InvalidCastException("The value is not of type Type");
        }

        protected override bool Not()
        {
            return false;
        }
    }
}
