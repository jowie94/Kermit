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
            object obj = Cast<object>(info[0]);
            KType ret = new KType(obj.GetType());
            info.ReturnValue.Value = ret;
        }
    }
}
