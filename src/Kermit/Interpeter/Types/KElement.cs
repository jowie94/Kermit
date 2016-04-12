using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Interpeter.Types
{
    public enum KType
    {
        Int,
        Float,
        Char,
        Bool,
        String,
        Object,
        Null
    }

    public abstract class KElement
    {
        public object Value
        {
            get { return GetType().GetField("Value").GetValue(this); }
            set { GetType().GetField("Value").SetValue(this, value); }
        }

        public KType Type { get; protected set; }

        public static bool operator ==(KElement obj1, KElement obj2)
        {
            return ReferenceEquals(obj1, obj2) || (obj1?.Equals(obj2) ?? false);
        }

        public static bool operator !=(KElement obj1, KElement obj2)
        {
            return !(obj1 == obj2);
        }

        public override string ToString()
        {
            return $"<({GetType().Name}) {GetType().GetField("Value").GetValue(this)}>";
        }

        public static bool operator !(KElement element)
        {
            return element.Not();
        }

        protected abstract bool Not();
    }
}
