namespace Kermit.Interpeter.Types
{
    /// <summary>
    /// Type representing a void
    /// </summary>
    public class KVoid : KObject
    {
        public new object Value => null;

        public override bool Equals(object obj)
        {
            return obj is KVoid;
        }

        public override int GetHashCode() => 0;

        protected override object GetValue()
        {
            return Value;
        }

        protected override void SetValue(object obj) {}

        protected override bool Not()
        {
            return true;
        }

        public override string ToString()
        {
            return "VOID";
        }
    }
}
