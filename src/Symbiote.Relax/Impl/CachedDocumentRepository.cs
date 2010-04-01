using System.Collections.Generic;
using StructureMap;

namespace Symbiote.Relax.Impl
{
    public class CachedDocumentRepository<TModel> 
        : BaseDocumentRepository<TModel>
        where TModel : class, ICouchDocument
    {
        protected ICouchCacheProvider _cache;
        protected IDocumentRepository<TModel> _repository;
        protected ICacheKeyBuilder _builder;

        public CachedDocumentRepository(ICouchConfiguration configuration, ICouchCommandFactory commandFactory, ICouchCacheProvider cacheProvider) 
            : base(configuration, commandFactory)
        {
            _cache = cacheProvider;
        }

        public CachedDocumentRepository(string configurationName, ICouchCacheProvider cacheProvider)
            : base(configurationName)
        {
            _cache = cacheProvider;
        }

        public override void DeleteDatabase()
        {
            _cache.DeleteAll<TModel>();
            base.DeleteDatabase();
        }

        public override void DeleteDocument<TKey, TRev>(TKey id, TRev rev)
        {
            _cache.Delete<TModel, TKey, TRev>(id, rev, base.DeleteDocument);
        }

        public override void DeleteDocument<TKey>(TKey id)
        {
            _cache.Delete<TModel, TKey>(id, base.DeleteDocument);
        }

        public override TModel Get<TKey, TRev>(TKey id, TRev revision)
        {
            return _cache.Get(id, revision, base.Get);
        }

        public override TModel Get<TKey>(TKey id)
        {
            return _cache.Get(id, base.Get);
        }

        public override IList<TModel> GetAll()
        {
            return _cache.GetAll(base.GetAll);
        }

        public override IList<TModel> GetAll(int pageSize, int pageNumber)
        {
            return _cache.GetAll(pageNumber, pageSize, base.GetAll);
        }

        public override void Save(TModel model)
        {
            _cache.Save(model, base.Save);
        }

        public override void Save(IEnumerable<TModel> list)
        {
            _cache.Save(list, base.Save);
        }
    }
}