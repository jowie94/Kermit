using System.Collections.Generic;

namespace Kermit.Parser
{
    public class FunctionSymbol : ScopedSymbol
    {
        public IDictionary<string, Symbol> Arguments { get; } = new Dictionary<string, Symbol>();

        public KermitAST BlockAST;

        public new string Name
        {
            get { return base.Name + '(' + string.Join(",", Arguments.Keys) + ')'; }
        }

        public FunctionSymbol(string name, IScope parentScope) : base(name, parentScope) {}

        public override IDictionary<string, Symbol> GetMembers()
        {
            return Arguments;
        }
    }
}
