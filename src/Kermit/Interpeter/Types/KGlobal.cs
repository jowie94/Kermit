using Interpeter.MemorySpaces;

namespace Interpeter.Types
{
    public class KGlobal : KLocal
    {
        private MemorySpace _space;

        public override KObject Value
        {
            get { return base.Value; }
            set
            {
                base.Value = value;
                _space[Name] = value;
            }
        }

        public KGlobal(string name, MemorySpace space) : base(name, space[name])
        {
            _space = space;
        }
    }
}
