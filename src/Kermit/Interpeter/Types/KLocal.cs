namespace Kermit.Interpeter.Types
{
    public class KLocal
    {
        private KObject _value;

        public virtual KObject Value // TODO: Maybe should be internal
        {
            get { return _value; }
            set { _value = value; }
        }

        public string Name { get; }

        public KLocal(string name, KObject value)
        {
            _value = value;
            Name = name;
        }

        internal KLocal(KObject value) : this("", value) { }
    }
}
