using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal;

namespace Parser
{
    class LocalScope : BaseScope
    {
        public LocalScope(IScope parent) : base(parent)
        {
            ScopeName = "local";
        }
    }
}
