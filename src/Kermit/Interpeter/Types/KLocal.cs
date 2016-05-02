namespace Kermit.Interpeter.Types
{
    /// <summary>
    /// Encapsulates a local variable
    /// </summary>
    public class KLocal
    {
        private KObject _value;

        public virtual KObject Value
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
