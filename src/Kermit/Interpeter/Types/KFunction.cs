using System;
using Kermit.Parser;

namespace Kermit.Interpeter.Types
{
    public class KFunction : KObject
    {
        internal new readonly FunctionSymbol Value;

        public bool IsNative { get; }

        public string Name => Value.Name;

        internal KFunction(FunctionSymbol fSymbol)
        {
            Value = fSymbol;
            IsNative = Value is NativeFunctionSymbol;
        }

        protected bool Equals(KFunction other)
        {
            return Equals(Value, other.Value) && IsNative == other.IsNative;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((KFunction) obj);
        }

        public override int GetHashCode()
        {
            return (base.GetHashCode() * 397) ^ IsNative.GetHashCode(); ;
        }

        protected override object GetValue()
        {
            return Value;
        }

        protected override void SetValue(object obj)
        {
            throw new InvalidOperationException("Can't set value of this type");
        }

        protected override bool Not()
        {
            return false;
        }
    }
}
