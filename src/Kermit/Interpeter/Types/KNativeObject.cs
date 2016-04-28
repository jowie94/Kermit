using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kermit.Interpeter.Types
{
    class KNativeObject : KObject
    {
        public new object Value;

        public KNativeObject(object obj)
        {
            Value = obj;
        }

        protected bool Equals(KNativeObject other)
        {
            return Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((KNativeObject) obj);
        }

        public override int GetHashCode() => base.GetHashCode();

        protected override object GetValue()
        {
            return Value;
        }

        protected override void SetValue(object obj)
        {
            Value = obj;
        }

        protected override bool Not()
        {
            return Value == null;
        }
    }
}
