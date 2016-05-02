using System;
using Kermit.Parser;

namespace Kermit.Interpeter
{
    /// <summary>
    /// Symbol for native objects
    /// </summary>
    class NativeSymbol : Symbol
    {
        internal readonly Type Type;

        public NativeSymbol(Type type) : this(type.Name, type) {}

        public NativeSymbol(string name, Type type) : base(name)
        {
            Type = type;
        }
    }
}
