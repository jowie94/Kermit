using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kermit.Interpeter.Types;

namespace Kermit.Interpeter.MemorySpaces
{
    public class MemorySpace
    {
        public string Name { get; }

        IDictionary<string, KLocal> _members = new Dictionary<string, KLocal>();
        protected IDictionary<string, KLocal> Members => _members;

        public MemorySpace(string name)
        {
            Name = name;
        }

        public virtual KLocal Get(string id)
        {
            KLocal o;
            return _members.TryGetValue(id, out o) ? o : null;
        }

        public virtual void Put(string id, KLocal value)
        {
            _members[id] = value;
        }

        public bool Contains(string id)
        {
            return _members.Keys.Contains(id);
        }

        public override string ToString()
        {
            return $"{Name}: {string.Join(";", _members.Select(x => $"{x.Key} = <{x.Value.Value.GetType().Name}> {x.Value.Value?.ToString()}"))}";
        }

        public KLocal this[string id]
        {
            get { return Get(id); }
            set { Put(id, value); }
        }
    }
}
