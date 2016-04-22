using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kermit.Interpeter.Types;

namespace Kermit.Interpeter.InternalFunctions
{
    class TypeOf : NativeFunction
    {
        public override void Execute(FunctionCallbackInfo info)
        {
            if (info.Length != 1)
                throw new ArgumentException("Expecting one parameter");
            KType ret = new KType(info[0].Value.GetType());
            info.ReturnValue.Value = ret;
        }
    }
}
