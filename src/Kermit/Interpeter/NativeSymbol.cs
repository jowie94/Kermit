using System;
using Kermit.Parser;

namespace Kermit.Interpeter
{
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
