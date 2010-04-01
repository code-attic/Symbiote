using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Symbiote.Core.Extensions;
using Symbiote.Eidetic;

namespace Symbiote.Relax.Impl
{
    public class EideticCacheProvider : ICouchCacheProvider
    {
        protected IRemember _cache;
        protected ICacheKeyBuilder _keyBuilder;
        protected ConcurrentDictionary<string, ConcurrentStack<string>> _crossReferences 
            = new ConcurrentDictionary<string, ConcurrentStack<string>>();
        
        public void AddCrossReference(string key, string cacheKey)
        {
            ConcurrentStack<string> list = null;
            if(!_crossReferences.TryGetValue(key, out list))
            {
                list = new ConcurrentStack<string>();
                _crossReferences[key] = list;
            }
            list.Push(cacheKey);
        }

        public void InvalidateItem<TModel>(string affectedKey)
            where TModel : class, ICouchDocument
        {
            ConcurrentStack<string> relatedKeys = null;
            if(_crossReferences.TryGetValue(affectedKey, out relatedKeys))
            {
                string key = null;
                while(relatedKeys.TryPop(out key))
                {
                    _cache.Remove(key);
                }
            }
        }

        public void Delete<TModel, TKey>(TKey key, Action<TKey> delete)
            where TModel : class, ICouchDocument
        {
            var cacheKey = _keyBuilder.GetKey<TModel>(key);
            InvalidateItem<TModel>(key.ToString());
            delete(key);
        }

        public void Delete<TModel, TKey, TRev>(TKey key, TRev rev, Action<TKey, TRev> delete)
            where TModel : class, ICouchDocument
        {
            var cacheKey = _keyBuilder.GetKey<TModel>(key, rev);
            InvalidateItem<TModel>(key.ToString());
            delete(key, rev);
        }

        public void DeleteAll<TModel>()
            where TModel : class, ICouchDocument
        {
            _crossReferences.Keys.ForEach(InvalidateItem<TModel>);
        }

        public TModel Get<TModel, TKey, TRev>(TKey key, TRev rev, Func<TKey, TRev, TModel> retrieve)
            where TModel : class, ICouchDocument
        {
            var cacheKey = _keyBuilder.GetKey<TModel>(key, rev);
            var result = _cache.Get<TModel>(cacheKey);
            if(result == null)
            {
                result = retrieve(key, rev);
                _cache.Store(StoreMode.Add, cacheKey, result);
                AddCrossReference(result.DocumentId, cacheKey);
            }
            return result;
        }

        public TModel Get<TModel, TKey>(TKey key, Func<TKey, TModel> retrieve)
            where TModel : class, ICouchDocument
        {
            var cacheKey = _keyBuilder.GetKey<TModel>(key);
            var result = _cache.Get<TModel>(cacheKey);
            if (result == null)
            {
                result = retrieve(key);
                _cache.Store(StoreMode.Add, cacheKey, result);
                AddCrossReference(result.DocumentId, cacheKey);
            }
            return result;
        }

        public IList<TModel> GetAll<TModel>(Func<IList<TModel>> retrieve)
            where TModel : class, ICouchDocument
        {
            var cacheKey = _keyBuilder.GetListKey<TModel>();
            var result = _cache.Get<IList<TModel>>(cacheKey);
            if (result == null)
            {
                result = retrieve();
                _cache.Store(StoreMode.Add, cacheKey, result);
                result.ForEach(x => AddCrossReference(x.DocumentId, cacheKey));
            }
            return result;
        }

        public IList<TModel> GetAll<TModel>(int pageNumber, int pageSize, Func<int, int, IList<TModel>> retrieve)
            where TModel : class, ICouchDocument
        {
            var cacheKey = _keyBuilder.GetListKey<TModel>(pageNumber, pageSize);
            var result = _cache.Get<IList<TModel>>(cacheKey);
            if (result == null)
            {
                result = retrieve(pageNumber, pageSize);
                _cache.Store(StoreMode.Add, cacheKey, result);
                result.ForEach(x => AddCrossReference(x.DocumentId, cacheKey));
            }
            return result;
        }

        public void Save<TModel>(TModel model, Action<TModel> save)
            where TModel : class, ICouchDocument
        {
            InvalidateItem<TModel>(model.DocumentId);
            save(model);
            CacheSavedModel(model);
        }

        protected void CacheSavedModel<TModel>(TModel model)
            where TModel : class, ICouchDocument
        {
            var simpleKey = _keyBuilder.GetKey<TModel>(model.DocumentId);
            var revKey = _keyBuilder.GetKey<TModel>(model.DocumentId, model.DocumentRevision);
            _cache.Store(StoreMode.Add, simpleKey, model);
            _cache.Store(StoreMode.Add, revKey, model);
        }

        public void Save<TModel>(IEnumerable<TModel> list, Action<IEnumerable<TModel>> save)
            where TModel : class, ICouchDocument
        {
            list.ForEach(x => InvalidateItem<TModel>(x.DocumentId));
            save(list);
            list.ForEach(CacheSavedModel);
        }

        public EideticCacheProvider(IRemember cache, ICacheKeyBuilder keyBuilder)
        {
            _cache = cache;
            _keyBuilder = keyBuilder;
        }
    }
}