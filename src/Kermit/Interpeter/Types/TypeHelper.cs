using System;
using System.Diagnostics.Contracts;

namespace Interpeter.Types
{
    [Pure]
    public class TypeHelper
    {
        public static T Cast<T>(KObject obj) where T : KObject
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (obj is T)
                return (T) obj;
            if (typeof(T).IsSubclassOf(typeof(KNumber)) && obj.GetType().IsSubclassOf(typeof(KNumber)))
                return (T) obj;
            throw new InvalidCastException($"Type {obj.GetType().Name} is not casteable to {typeof(T).Name}");
        }

        public static bool Is<T>(KObject obj)
        {
            return obj is T || obj.Value is T;
        }

        public static KString ToString(KObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            return new KString(obj.ToString());
        }

        public static KInt ToInt(KObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            KNumber number = obj as KNumber;
            if (number != null)
                return new KInt(number.ToInt());
            KString str = obj as KString;
            if (str != null)
            {
                int res;
                if (int.TryParse(str, out res))
                    return new KInt(res);
            }
            throw new InvalidCastException($"Can't convert {obj.GetType().Name} to int");
        }

        public static KFloat ToFloat(KObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            KNumber number = obj as KNumber;
            if (number != null)
                return new KFloat(number.ToFloat());
            KString str = obj as KString;
            if (str != null)
            {
                float res;
                if (float.TryParse(str, out res))
                    return new KFloat(res);
            }
            throw new InvalidCastException($"Can't convert {obj.GetType().Name} to float");
        }

        public static KBool ToBool(KObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (obj is KBool)
                return (KBool)obj;
            if (obj is KString)
                return ((KString) obj).Value.Length > 0;
            if (obj is KNumber)
                return ((KNumber) obj).ToInt() != 0;
            if (obj is KChar)
                return ((KChar) obj).Value != null;
            throw new InvalidCastException($"Can't convert {obj.GetType().Name} to bool");
        }
    }
}
