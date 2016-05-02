namespace Kermit.Interpeter
{
    internal class GlobalScope : ComitableScope
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
