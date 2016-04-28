﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermit.Interpeter.Types
{
    public class KArray : KObject
    {
        public new List<KObject> Value = new List<KObject>();

        public KArray(params KObject[] args)
        {
            Value.AddRange(args);
        }

        public KObject this[int el]
        {
            get { return Value[el]; }
            set { Value[el] = value; }
        }

        protected bool Equals(KArray other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((KArray) obj);
        }

        public override int GetHashCode() => base.GetHashCode();

        protected override object GetValue()
        {
            return Value;
        }

        protected override void SetValue(object obj)
        {
            if (obj is List<KObject>)
                Value = (List<KObject>) obj;
            else
                throw new InvalidCastException("The value is not of type List");
        }

        protected override bool Not()
        {
            return Value.Count == 0;
        }
    }
}