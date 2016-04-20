using Interpeter.MemorySpaces;

namespace Interpeter.Types
{
    public class KGlobal : KLocal
    {
        private MemorySpace _space;
        private string _innerName;

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
