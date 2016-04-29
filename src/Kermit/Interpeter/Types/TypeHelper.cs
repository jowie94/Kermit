﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Diagnostics.Contracts;
using Kermit.Interpeter.Exceptions;

namespace Kermit.Interpeter.Types
{
    [Pure]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class TypeHelper
    {
        public static KObject ToKObject(object obj)
        {
            if (obj is string)
                return new KString((string) obj);
            if (obj is int)
                return new KInt((int) obj);
            if (obj is float)
                return new KFloat((float) obj);
            if (obj is char)
                return new KChar((char) obj);
            if (obj is bool)
                return new KBool((bool) obj);
            return new KNativeObject(obj);
        }

        public static T Cast<T>(KObject obj) where T : KObject
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (obj is T)
                return (T) obj;
            ThrowHelper.TypeError($"Type {obj.GetType().Name} is not casteable to {typeof(T).Name}");
            return null;
        }

        public static List<KLocal> ToParameterList(params object[] parameters)
        {
            return ToParameterList(parameters.ToList());
        }

        public static List<KLocal> ToParameterList(IEnumerable<object> parameters)
        {
            return parameters.Select(x => new KLocal(ToKObject(x))).ToList();
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
            ThrowHelper.TypeError($"Can't convert {obj.GetType().Name} to int");
            return null;
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
            ThrowHelper.TypeError($"Can't convert {obj.GetType().Name} to float");
            return null;
        }

        public static KBool ToBool(KObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (obj is KBool)
                return (KBool) obj;
            if (obj is KString)
                return ((KString) obj).Value.Length > 0;
            if (obj is KNumber)
                return ((KNumber) obj).ToInt() != 0;
            if (obj is KChar)
                return true;
            ThrowHelper.TypeError($"Can't convert {obj.GetType().Name} to bool");
            return null;
        }
    }
}
