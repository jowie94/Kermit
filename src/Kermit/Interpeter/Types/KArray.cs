using System.Collections.Generic;
using System.Linq;

namespace Kermit.Interpeter.Types
{
    public class KArray : KObject
    {
        public new List<KObject> Value = new List<KObject>();

        public KArray(params KObject[] args)
        {
            Value.AddRange(args);
        }

        public KObject this[int el]
        {
            get { return Value[el]; }
            set { Value[el] = value; }
        }

        protected override bool Not()
        {
            return Value.Count == 0;
        }
    }
}