using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kermit.Parser;

namespace KermitVerifier
{
    class GlobalScope : BaseScope
    {
        public GlobalScope() : base(null)
        {
            ScopeName = "global";
        }
    }
}
