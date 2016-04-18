using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interpeter.Types;

namespace Interpeter.InternalFunctions
{
    internal class Write : KFunction
    {
        public override void Execute(List<KVariable> variables)
        {
            string msg = "";
            if (variables.Count > 0)
                msg = variables[0].Value.ToString();
            if (variables.Count > 1)
                msg = string.Format(msg, variables.ToList());
            Console.WriteLine(msg);
        }
    }
}
