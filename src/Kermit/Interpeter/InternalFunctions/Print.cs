using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpeter.InternalFunctions
{
    internal class Print : KFunction
    {
        public override void Execute()
        {
            Console.WriteLine("Hello world");
        }
    }
}
