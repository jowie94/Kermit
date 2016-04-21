using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Interpeter.Types
{
    public abstract class KObject
    {
        public virtual object Value
        {
            get { return GetType().GetField("Value").GetValue(this); }
            set { GetType().GetField("Value").SetValue(this, value); }
        }

        public bool IsVoid => Is<KVoid>();

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

        public KObject GetInnerField(string name)
        {
            object obj = Value;
            PropertyInfo pinfo = obj.GetType()
                .GetProperty(name,
                    BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            if (pinfo == null)
                return null;
            // TODO: check fields
            return TypeHelper.ToKObject(pinfo.GetValue(obj));
        }

        public bool SetInnerField(string name, KObject value)
        {
            object obj = Value;
            PropertyInfo pinfo = obj.GetType()
                .GetProperty(name,
                    BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            if (pinfo == null)
                return false;
            // TODO: check fields
            pinfo.SetValue(obj, value.Value);
            return true;
        }

        public KObject CallInnerFunction(string name, object[] parameters)
        {
            object obj = Value;
            Type[] types = parameters.Select(x => x.GetType()).ToArray(); // TODO: Border case, array parameter
            MethodInfo info = obj.GetType().GetMethod(name, types);
            if (info == null)
                return null;
            object ret = info.Invoke(obj, parameters);
            if (info.ReturnType == typeof(void))
                return new KVoid();
            return TypeHelper.ToKObject(ret);
        }

        protected abstract bool Not();
    }
}
