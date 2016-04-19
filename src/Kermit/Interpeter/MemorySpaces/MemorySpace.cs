using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interpeter.Types;

namespace Interpeter.MemorySpaces
{
    public class MemorySpace
    {
        public string Name { get; }

        IDictionary<string, KObject> _members = new Dictionary<string, KObject>();
        protected IDictionary<string, KObject> Members => _members;

        public MemorySpace(string name)
        {
            Name = name;
        }

        public virtual KObject Get(string id)
        {
            KObject o;
            return _members.TryGetValue(id, out o) ? o : null;
        }

        public virtual void Put(string id, KObject value)
        {
            _members[id] = value;
        }

        public bool Contains(string id)
        {
            return _members.Keys.Contains(id);
        }

        public override string ToString()
        {
            return $"{Name}: {string.Join(";", _members.Select(x => x.Key + "=" + x.Value?.ToString()))}";
        }

        public KObject this[string id]
        {
            get { return Get(id); }
            set { Put(id, value); }
        }
    }
}
