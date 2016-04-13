using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpeter
{
    public abstract class KFunction
    {
        public string Name { get; internal set; }

        public abstract void Execute();
    }
}
