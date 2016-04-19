using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interpeter.MemorySpaces;

namespace Interpeter.Types
{
    public class KVariable
    {
        private string _name;
        private MemorySpace _space;
        private KObject _value;

        public KObject Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _space[_name] = value;
            }
        }

        internal KVariable(string name, MemorySpace space)
        {
            _name = name;
            _space = space;
            _value = _space[name];
        }

    }
}
