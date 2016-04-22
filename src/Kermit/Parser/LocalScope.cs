namespace Kermit.Parser
{
    class LocalScope : BaseScope
    {
        public LocalScope(IScope parent) : base(parent)
        {
            ScopeName = "local";
        }
    }
}
