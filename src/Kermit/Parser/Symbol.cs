using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public class Symbol
    {
        /// <summary>
        /// Symbol's name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Symbol's containing scope
        /// </summary>
        public IScope Scope;

        public Symbol(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            string s = "";
            if (Scope != null) s = Scope.ScopeName + ".";
            return s + Name;
        }
    }
}
