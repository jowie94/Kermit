using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpeter.Types
{
    public class KVoid : KObject
    {
        public new object Value => null;

        protected override bool Not()
        {
            return true;
        }

        public override string ToString()
        {
            return "VOID";
        }
    }
}
