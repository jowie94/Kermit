using Interpeter.MemorySpaces;

namespace Interpeter.Types
{
    public class KGlobal : KLocal
    {
        private string _name;
        private MemorySpace _space;

        public override KObject Value
        {
            get { return base.Value; }
            set
            {
                base.Value = value;
                _space[_name] = value;
            }
        }

        public KGlobal(string name, MemorySpace space) : base(space[name])
        {
            _name = name;
            _space = space;
        }
    }
}
