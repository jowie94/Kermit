using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpeter.Types
{
    public abstract class KNumber : KElement
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
    }
}
