﻿using System;

namespace Kermit.Interpeter.Types
{
    /// <summary>
    /// Type representing a float
    /// </summary>
    public class KFloat : KNumber
    {
        public new float Value;

        public KFloat(float value)
        {
            Value = value;
        }

        private bool Equals(KFloat other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((KFloat) obj);
        }

        public override int GetHashCode() => base.GetHashCode();

        public override int ToInt()
        {
            return Convert.ToInt32(Value);
        }

        public override float ToFloat()
        {
            return Value;
        }

        protected override object GetValue()
        {
            return Value;
        }

        protected override void SetValue(object obj)
        {
            if (obj is float)
                Value = (float) obj;
            else
                throw new InvalidCastException("The value is not of type float");
        }
    }
}
