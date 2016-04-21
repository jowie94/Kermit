using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interpeter.Types;

namespace Interpeter.InternalFunctions
{
    class List : NativeFunction
    {
        public override void Execute(FunctionCallbackInfo info)
        {
            info.ReturnValue.Value = TypeHelper.ToKObject(new List<object>());
        }
    }
}
