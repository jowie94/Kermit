namespace Kermit.Parser
{
    /// <summary>
    /// Local scope for functions
    /// </summary>
    class LocalScope : BaseScope
    {
        public LocalScope(IScope parent) : base(parent)
        {
            ScopeName = "local";
        }
    }
}
