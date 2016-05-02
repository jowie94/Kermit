using System.Collections.Generic;

namespace Kermit.Parser
{
    /// <summary>
    /// Symbol representing a function
    /// </summary>
    public class FunctionSymbol : ScopedSymbol
    {
        public IDictionary<string, Symbol> Arguments { get; } = new Dictionary<string, Symbol>();

        /// <summary>
        /// AST with the function
        /// </summary>
        public KermitAST BlockAst;

        /// <summary>
        /// Name of the function + arguments
        /// </summary>
        public new string Name => base.Name + '(' + string.Join(",", Arguments.Keys) + ')';

        public FunctionSymbol(string name, IScope parentScope) : base(name, parentScope) {}

        /// <summary>
        /// Get arguments of the function
        /// </summary>
        /// <returns></returns>
        public override IDictionary<string, Symbol> GetMembers()
        {
            return Arguments;
        }
    }
}
