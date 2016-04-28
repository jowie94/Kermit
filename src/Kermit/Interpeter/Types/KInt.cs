using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kermit.Interpeter.Types
{
    public class KInt : KNumber
    {
        public new int Value;

        public KInt(int value)
        {
            Value = value;
        }

        protected bool Equals(KInt other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((KInt) obj);
        }

        public override int GetHashCode() => base.GetHashCode();

        public override int ToInt()
        {
            return Value;
        }

        public override float ToFloat()
        {
            return Value;
        }

        public static explicit operator KBool(KInt num)
        {
            return num.ToInt() != 0;
        }

        protected override object GetValue()
        {
            return Value;
        }

        protected override void SetValue(object obj)
        {
            if (obj is int)
                Value = (int) obj;
            else
                throw new InvalidCastException("The value is not of type int");
        }
    }
}
