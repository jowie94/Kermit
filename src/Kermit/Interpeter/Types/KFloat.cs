﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kermit.Interpeter.Types
{
    public class KFloat : KNumber
    {
        public new float Value;

        public KFloat(float value)
        {
            Value = value;
        }

        protected bool Equals(KFloat other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((KFloat) obj);
        }

        public override int ToInt()
        {
            return Convert.ToInt32(Value);
        }

        public override float ToFloat()
        {
            return Value;
        }
    }
}
