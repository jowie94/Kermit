using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kermit.Interpeter.Exceptions;

namespace Kermit.Interpeter.Types
{
    /// <summary>
    /// Basic object. All objects must inherit this type
    /// </summary>
    public abstract class KObject
    {
        /// <summary>
        /// The value real of the object
        /// </summary>
        public virtual object Value
        {
            get { return GetValue(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// If the object is void
        /// </summary>
        public bool IsVoid => Is<KVoid>();

        /// <summary>
        /// If the object is a number
        /// </summary>
        public bool IsNumber => Is<KNumber>();

        /// <summary>
        /// If the object is an integer
        /// </summary>
        public bool IsInt => Is<KInt>();

        /// <summary>
        /// If the object is a float
        /// </summary>
        public bool IsFloat => Is<KFloat>();

        /// <summary>
        /// If the object is a char
        /// </summary>
        public bool IsChar => Is<KChar>();

        /// <summary>
        /// If the object is a string
        /// </summary>
        public bool IsString => Is<KString>();

        public bool IsNative => Is<KNativeObject>();

        public virtual IList<string> Methods => GetValue().GetType().GetMethods().Select(FormatMethod).ToList();

        public virtual IList<string> Properties => GetValue().GetType().GetProperties().Select(x => $"{x.PropertyType.Name} {x.Name}").ToList();

        /// <summary>
        /// Generic Type checker
        /// </summary>
        /// <typeparam name="T">The type to check</typeparam>
        /// <returns>If the object is of thar type</returns>
        public bool Is<T>() => TypeHelper.Is<T>(this);

        /// <summary>
        /// Cast the object to another type
        /// </summary>
        /// <typeparam name="T">The type to cast</typeparam>
        /// <returns>The object casted</returns>
        public T Cast<T>() where T : KObject => TypeHelper.Cast<T>(this);

        public static bool operator ==(KObject obj1, KObject obj2)
        {
            return ReferenceEquals(obj1, obj2) || (obj1?.Equals(obj2) ?? false);
        }

        public static bool operator !=(KObject obj1, KObject obj2)
        {
            return !(obj1 == obj2);
        }

        public abstract override bool Equals(object obj);

        public override int GetHashCode() => Value?.GetHashCode() ?? 0;

        public override string ToString()
        {
            return Value.ToString();
        }

        public static bool operator !(KObject element)
        {
            return element.Not();
        }

        /// <summary>
        /// Gets the value of an inner field
        /// </summary>
        /// <param name="name">The name to search</param>
        /// <returns>The value of the field or null if not found</returns>
        public virtual KObject GetInnerField(string name)
        {
            object obj = Value;
            Type oType = obj.GetType();
            BindingFlags flags = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Static |
                                 BindingFlags.Public;
            PropertyInfo pinfo = oType.GetProperty(name, flags);
            KObject res = null;
            if (pinfo != null)
                res = TypeHelper.ToKObject(pinfo.GetValue(obj));
            else
            {
                FieldInfo finfo = oType.GetField(name, flags);
                if (finfo != null)
                    res = TypeHelper.ToKObject(finfo.GetValue(obj));
            }
            return res;
        }

        /// <summary>
        /// Sets the value of an inner field
        /// </summary>
        /// <param name="name">The field to be set</param>
        /// <param name="value">The new value</param>
        /// <returns>true or false depending if the field was found</returns>
        public virtual bool SetInnerField(string name, KObject value)
        {
            object obj = Value;
            Type oType = obj.GetType();
            BindingFlags flags = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Static |
                                 BindingFlags.Public;
            PropertyInfo pinfo = oType.GetProperty(name, flags);
            if (pinfo != null)
                pinfo.SetValue(obj, value.Value);
            else
            {
                FieldInfo finfo = oType.GetField(name, flags);
                if (finfo != null)
                    finfo.SetValue(name, flags);
                else return false;
            }
            return true;
        }

        /// <summary>
        /// Calls an inner function
        /// </summary>
        /// <param name="name">The function to call</param>
        /// <param name="parameters">The parameters to be passed</param>
        /// <returns>The value returned by the function</returns>
        public virtual KObject CallInnerFunction(string name, object[] parameters)
        {
            object obj = Value;
            Type[] types = parameters.Select(x => x.GetType()).ToArray(); // TODO: Border case, array parameter (delayed)
            MethodInfo info = obj.GetType().GetMethod(name, types);
            if (info == null)
                return null;
            object ret = null;
            try
            {
                ret = info.Invoke(obj, parameters);
            }
            catch (TargetInvocationException e)
            {
                ThrowHelper.InterpreterException("Exception thrown by inner call\n" + e.InnerException.Message, e.InnerException);
            }
            if (info.ReturnType == typeof(void))
                return new KVoid();
            return TypeHelper.ToKObject(ret);
        }

        protected static string FormatMethod(MethodInfo method)
        {
            return
                $"{method.ReturnType.Name} {method.Name}({string.Join(",", method.GetParameters().Select(x => x.ParameterType.Name))})";
        }

        /// <summary>
        /// Get the real value of the super-type
        /// </summary>
        /// <returns>The super-type value</returns>
        protected abstract object GetValue();

        /// <summary>
        /// Sets the value of the super-type
        /// </summary>
        /// <param name="obj">The new value</param>
        protected abstract void SetValue(object obj);

        /// <summary>
        /// Returns the not of the super-type
        /// </summary>
        /// <returns>The not of the super-type</returns>
        protected abstract bool Not();
    }
}
