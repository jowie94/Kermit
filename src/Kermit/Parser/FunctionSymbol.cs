using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public class FunctionSymbol : ScopedSymbol
    {
        public IDictionary<string, Symbol> Arguments { get; } = new Dictionary<string, Symbol>();

        public KermitAST BlockAST;

        public new string Name
        {
            get { return base.Name + '(' + Arguments.Keys.ToString() + ')'; }
        }

        public FunctionSymbol(string name, IScope parentScope) : base(name, parentScope) {}

        public override IDictionary<string, Symbol> GetMembers()
        {
            return Arguments;
        }
    }
}
