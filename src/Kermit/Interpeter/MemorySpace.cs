using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpeter
{
    public class MemorySpace
    {
        string _name;

        IDictionary<string, object> _members = new Dictionary<string, object>();

        public MemorySpace(string name)
        {
            _name = name;
        }

        public object Get(string id)
        {
            object o;
            return _members.TryGetValue(id, out o) ? o : null;
        }

        public void Put(string id, object value)
        {
            _members[id] = value;
        }

        public override string ToString()
        {
            return $"{_name}: {string.Join(";", _members.Select(x => x.Key + "=" + x.Value?.ToString()))}";
        }

        public object this[string id]
        {
            get { return Get(id); }
            set { Put(id, value); }
        }
    }
}
