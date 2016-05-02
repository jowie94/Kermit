using System.Collections.Generic;
using System.Linq;
using Kermit.Interpeter.Types;

namespace Kermit.Interpeter.MemorySpaces
{
    /// <summary>
    /// Generic memory space
    /// </summary>
    public class MemorySpace
    {
        /// <summary>
        /// Name of the memory space
        /// </summary>
        public string Name { get; }

        readonly IDictionary<string, KLocal> _members = new Dictionary<string, KLocal>();
        protected IDictionary<string, KLocal> Members => _members;

        public MemorySpace(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the variable with name <paramref name="id"/>
        /// </summary>
        /// <param name="id">The name of the variable</param>
        /// <returns>The value or null if not found</returns>
        public virtual KLocal Get(string id)
        {
            KLocal o;
            return _members.TryGetValue(id, out o) ? o : null;
        }

        /// <summary>
        /// Adds or updates a variable in the space
        /// </summary>
        /// <param name="id">The name of the variable</param>
        /// <param name="value">The new value</param>
        public virtual void Put(string id, KLocal value)
        {
            _members[id] = value;
        }

        /// <summary>
        /// Checks if a variable exists in the space
        /// </summary>
        /// <param name="id">The name of the variable</param>
        /// <returns>true or false depending if the variable exists</returns>
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
