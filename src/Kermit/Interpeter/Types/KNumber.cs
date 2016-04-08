using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpeter.Types
{
    public abstract class KNumber : KElement, IComparable
    {
        public abstract int ToInt();

        public abstract float ToFloat();

        public static KNumber operator +(KNumber x, KNumber y)
        {
            if (x.Type == KType.Float || y.Type == KType.Float)
                return new KFloat(x.ToFloat() + y.ToFloat());
            return new KInt(x.ToInt() + y.ToInt());
        }

        public static KNumber operator -(KNumber x, KNumber y)
        {
            if (x.Type == KType.Float || y.Type == KType.Float)
                return new KFloat(x.ToFloat() - y.ToFloat());
            return new KInt(x.ToInt() + y.ToInt());
        }

        public static KNumber operator *(KNumber x, KNumber y)
        {
            if (x.Type == KType.Float || y.Type == KType.Float)
                return new KFloat(x.ToFloat() * y.ToFloat());
            return new KInt(x.ToInt() * y.ToInt());
        }

        public static KNumber operator /(KNumber x, KNumber y)
        {
            if (x.Type == KType.Float || y.Type == KType.Float)
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

        public int CompareTo(object obj)
        {
            KNumber num = obj as KNumber;
            if (num != null)
            {
                return ((IComparable) Value).CompareTo(num.Value);
            }
            throw new ArgumentException("Object is not a number");
        }
    }
}
