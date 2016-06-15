namespace Kermit.Interpeter
{
    internal class GlobalScope : CommitableScope
    {
        /// <summary>
        /// Represents the global scope
        /// </summary>
        internal GlobalScope() : base(null)
        {
            ScopeName = "global";
        }
    }
}
