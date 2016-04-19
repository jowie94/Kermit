using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

    public abstract class KObject
    {
        public virtual object Value
        {
            get { return GetType().GetField("Value").GetValue(this); }
            set { GetType().GetField("Value").SetValue(this, value); }
        }

        public bool IsNumber => Is<KNumber>();

        public bool IsInt => Is<KInt>();

        public bool IsFloat => Is<KFloat>();

        public bool IsChar => Is<KChar>();

        public bool IsString => Is<KString>();

        //public bool IsExternal => TypeHelper.Is<KExternal>(this);

        public bool Is<T>() => TypeHelper.Is<T>(this);

        public T Cast<T>() where T : KObject => TypeHelper.Cast<T>(this);

        public static bool operator ==(KObject obj1, KObject obj2)
        {
            return ReferenceEquals(obj1, obj2) || (obj1?.Equals(obj2) ?? false);
        }

        public static bool operator !=(KObject obj1, KObject obj2)
        {
            return !(obj1 == obj2);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static bool operator !(KObject element)
        {
            return element.Not();
        }

        protected abstract bool Not();
    }
}
