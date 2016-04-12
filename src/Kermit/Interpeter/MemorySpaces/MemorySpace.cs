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
        string _name;

        IDictionary<string, KElement> _members = new Dictionary<string, KElement>();

        public MemorySpace(string name)
        {
            _name = name;
        }

        public virtual KElement Get(string id)
        {
            KElement o;
            return _members.TryGetValue(id, out o) ? o : null;
        }

        public virtual void Put(string id, KElement value)
        {
            _members[id] = value;
        }

        public bool Contains(string id)
        {
            return _members.Keys.Contains(id);
        }

        public override string ToString()
        {
            return $"{_name}: {string.Join(";", _members.Select(x => x.Key + "=" + x.Value?.ToString()))}";
        }

        public KElement this[string id]
        {
            get { return Get(id); }
            set { Put(id, value); }
        }
    }
}
