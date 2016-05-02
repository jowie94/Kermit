using System;
using Kermit.Interpeter.Exceptions;

namespace Kermit.Interpeter.Types
{
    /// <summary>
    /// Super-type of numbers
    /// </summary>
    public abstract class KNumber : KObject, IComparable
    {
        /// <summary>
        /// Converts the number to an integer
        /// </summary>
        /// <returns>The integer represetation</returns>
        public abstract int ToInt();

        /// <summary>
        /// Converts the number to a float
        /// </summary>
        /// <returns>The float representation</returns>
        public abstract float ToFloat();

        public static KNumber operator +(KNumber x, KNumber y)
        {
            if (x.IsFloat || y.IsFloat)
                return new KFloat(x.ToFloat() + y.ToFloat());
            return new KInt(x.ToInt() + y.ToInt());
        }

        public static KNumber operator -(KNumber x, KNumber y)
        {
            if (x.IsFloat || y.IsFloat)
                return new KFloat(x.ToFloat() - y.ToFloat());
            return new KInt(x.ToInt() + y.ToInt());
        }

        public static KNumber operator *(KNumber x, KNumber y)
        {
            if (x.IsFloat || y.IsFloat)
                return new KFloat(x.ToFloat() * y.ToFloat());
            return new KInt(x.ToInt() * y.ToInt());
        }

        public static KNumber operator /(KNumber x, KNumber y)
        {
            if (x.IsFloat || y.IsFloat)
                return new KFloat(x.ToFloat() / y.ToFloat());
            return new KInt(x.ToInt() / y.ToInt());
        }

        public static implicit operator KNumber(int num)
        {
            return new KInt(num);
        }

        public static implicit operator KNumber(float num)
        {
            return new KFloat(num);
        }

        public static implicit operator int(KNumber num)
        {
            return num.ToInt();
        }

        public static implicit operator float(KNumber num)
        {
            return num.ToFloat();
        }

        protected override bool Not()
        {
            return !(KBool)this;
        }

        public int CompareTo(object obj)
        {
            KNumber num = obj as KNumber;
            if (num != null)
                return ((IComparable) Value).CompareTo(num.Value);
            ThrowHelper.TypeError("Object is not a number");
            return 0;
        }
    }
}
