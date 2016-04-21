using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpeter.Types
{
    class KNativeObject : KObject
    {
        public new object Value;

        public KNativeObject(object obj)
        {
            Value = obj;
        }

        protected override bool Not()
        {
            return Value == null;
        }
    }
}
