using System;
using Kermit.Interpeter.Types;

namespace Kermit.Interpeter.MemorySpaces
{
    /// <summary>
    /// Scoped memory space including a parent
    /// </summary>
    class ScopeSpace : MemorySpace
    {
        private readonly MemorySpace _parentSpace;

        public ScopeSpace(string name, MemorySpace parentSpace) : base(name)
        {
            if (parentSpace == null)
                throw new ArgumentNullException(nameof(parentSpace));
            _parentSpace = parentSpace;
        }

        public override KLocal Get(string id)
        {
            KLocal ret = base.Get(id) ?? _parentSpace[id];
            return ret;
        }

        public override void Put(string id, KLocal value)
        {
            if (Contains(id) || !_parentSpace.Contains(id))
                base.Put(id, value);
            else
                _parentSpace[id] = value;
        }
    }
}
