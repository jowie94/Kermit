using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Diagnostics.Contracts;
using Antlr.Runtime.Tree;
using Kermit.Interpeter.Exceptions;

namespace Kermit.Interpeter.Types
{
    /// <summary>
    /// Class with helper functions for internal types
    /// </summary>
    [Pure]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class TypeHelper
    {
        /// <summary>
        /// Convert an object to a internal object
        /// </summary>
        /// <param name="obj">The object to be converted</param>
        /// <returns>A new internal object encapsulating the real object</returns>
        public static KObject ToKObject(object obj)
        {
            if (obj is KObject)
                return (KObject) obj;
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

        /// <summary>
        /// Cast an internal object to another
        /// </summary>
        /// <typeparam name="T">The type to be casted</typeparam>
        /// <param name="obj">The object to be casted</param>
        /// <returns>The object casted, an argument exception if the object is null or an interpreter exception if it is not casteable</returns>
        public static T Cast<T>(dynamic obj) where T : KObject
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (!(obj is KObject))
                throw new ArgumentException("Object is not a KObject");
            try
            {
                return (T) obj;
            }
            catch (Exception)
            {
                ThrowHelper.TypeError($"Type {obj.GetType().Name} is not casteable to {typeof(T).Name}");
            }
            return null;
        }

        /// <summary>
        /// Converts an array of parameters to a list of parameters
        /// </summary>
        /// <param name="parameters">The parameters to be converted</param>
        /// <returns>The list of parameters converted</returns>
        public static List<KLocal> ToParameterList(params object[] parameters)
        {
            return ToParameterList(parameters.ToList());
        }

        /// <summary>
        /// Converts an IEnumerable of parameters to a list of parameters
        /// </summary>
        /// <param name="parameters">The parameters to be converted</param>
        /// <returns>The list of parameters converted</returns>
        public static List<KLocal> ToParameterList(IEnumerable<object> parameters)
        {
            return parameters.Select(x => new KLocal(ToKObject(x))).ToList();
        }

        /// <summary>
        /// Checks if the object or the inner object is of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to be checked</typeparam>
        /// <param name="obj">The object to be checked</param>
        /// <returns>true or false if the object is of type <typeparamref name="T"/></returns>
        public static bool Is<T>(KObject obj)
        {
            return obj is T || obj.Value is T;
        }

        public static KArray CreateArray(params object[] elements)
        {
            return CreateArray(elements.ToList());
        }

        public static KArray CreateArray(IEnumerable<object> elements)
        {
            return new KArray(elements.Select(ToKObject).ToArray());
        }

        /// <summary>
        /// Converts an object to a string
        /// </summary>
        /// <param name="obj">The object to convert to string</param>
        /// <returns>The object converted to string</returns>
        public static KString ToString(KObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            return new KString(obj.ToString());
        }

        /// <summary>
        /// Converts an object to integer
        /// </summary>
        /// <param name="obj">The object to be converted</param>
        /// <returns>The object converted</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="obj"/> is null</exception>
        /// <exception cref="InterpreterException">If <paramref name="obj"/> can't be converted to int</exception>
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

        /// <summary>
        /// Converts an object to float
        /// </summary>
        /// <param name="obj">The object to be converted</param>
        /// <returns>The object converted</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="obj"/> is null</exception>
        /// <exception cref="InterpreterException">If <paramref name="obj"/> can't be converted to float</exception>
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

        /// <summary>
        /// Converts an object to boolean
        /// </summary>
        /// <param name="obj">The object to be converted</param>
        /// <returns>The object converted</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="obj"/> is null</exception>
        /// <exception cref="InterpreterException">If <paramref name="obj"/> can't be converted to boolean</exception>
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
            try
            {
                return obj.Cast<KBool>(); // If conversion is not known, try to use the internal casting function
            }
            catch
            {
                ThrowHelper.TypeError($"Can't convert {obj.GetType().Name} to bool");
                return null;
            }
        }
    }
}
