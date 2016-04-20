using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interpeter.Types;

namespace Interpeter.InternalFunctions
{
    internal class Write : NativeFunction
    {
        public override void Execute(FunctionCallbackInfo info)
        {
            string msg = "";
            if (info.Length > 0)
                msg = TypeHelper.ToString(info[0].Value);
            if (info.Length > 1)
            {
                object[] obj = new object[info.Length - 1];
                for (int i = 1; i < info.Length; ++i)
                    obj[i - 1] = info[i].Value.Value;
                msg = string.Format(msg, obj);
            }
                
            Console.WriteLine(msg);
        }
    }
}
