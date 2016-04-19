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
                msg = TypeHelper.ToString(variables[0].Value);
            if (variables.Count > 1)
                msg = string.Format(msg, variables.Skip(1).Select(x => x.Value.Value).ToArray());
            Console.WriteLine(msg);
        }
    }
}
