using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kermit.Interpeter.Types
{
    class KType : KObject
    {
        public new Type Value;

        public KType(Type value)
        {
            Value = value;
        }

        protected bool Equals(KType other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((KType) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        protected override bool Not()
        {
            return false;
        }
    }
}
