using Kermit.Parser;

namespace Kermit.Interpeter
{
    internal class GlobalScope : ComitableScope
    {
        internal GlobalScope() : base(null)
        {
            ScopeName = "global";
        }
    }
}
