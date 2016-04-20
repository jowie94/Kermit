namespace Interpeter.Types
{
    public class KLocal
    {
        private KObject _value;

        public virtual KObject Value
        {
            get { return _value; }
            set { _value = value; }
        }

        internal KLocal(KObject value)
        {
            _value = value;
        }

    }
}
