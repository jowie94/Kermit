using Kermit.Interpeter.MemorySpaces;

namespace Kermit.Interpeter.Types
{
    /// <summary>
    /// Encapsulates a global variable and updates the value in the memory space when set
    /// </summary>
    public class KGlobal : KLocal
    {
        private readonly MemorySpace _space;
        private readonly string _innerName;

        public override KObject Value
        {
            get { return base.Value; }
            set
            {
                base.Value = value;
                _space[_innerName].Value = value;
            }
        }

        public KGlobal(string name, MemorySpace space) : this(name, name, space) { }

        public KGlobal(string name, string innerName, MemorySpace space) : base(name, space[innerName].Value)
        {
            _space = space;
            _innerName = innerName;
        }
    }
}
