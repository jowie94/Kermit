namespace Kermit.Parser
{
    /// <summary>
    /// Symbol representation
    /// </summary>
    public class Symbol
    {
        /// <summary>
        /// Symbol's name
        /// </summary>
        public string Name { get; }

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
