using System.Collections.Generic;

namespace Symbiote.Hibernate
{
    public class InMemoryContext : IContext
    {
        private Dictionary<string, object> _hash = new Dictionary<string, object>();

        public bool Contains(string key)
        {
            return _hash.ContainsKey(key);
        }

        public void Set(string key, object value)
        {
            _hash[key] = value;
        }

        public object Get(string key)
        {
            object value = null;
            _hash.TryGetValue(key, out value);
            return value;
        }
    }
}