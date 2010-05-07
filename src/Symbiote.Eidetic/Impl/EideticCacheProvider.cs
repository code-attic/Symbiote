using Symbiote.Core.Cache;

namespace Symbiote.Eidetic.Impl
{
    public class EideticCacheProvider : ICacheProvider
    {
        protected IRemember _remember;

        public void Store<T>(string key, T value)
        {
            _remember.Store(StoreMode.Set, key, value);
        }

        public T Get<T>(string key)
        {
            return _remember.Get<T>(key);
        }

        public void Remove(string key)
        {
            _remember.Remove(key);
        }

        public EideticCacheProvider(IRemember remember)
        {
            _remember = remember;
        }
    }
}
