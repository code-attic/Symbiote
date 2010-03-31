using System;
using System.Collections.Generic;
using Symbiote.Core.Extensions;
using Symbiote.Eidetic;

namespace Symbiote.Relax.Impl
{
    public class CachedDocumentRepository 
        : BaseDocumentRepository<Guid, string>, IDocumentRepository
    {
        protected IRemember _cache;

        public CachedDocumentRepository(ICouchConfiguration configuration, ICouchCommandFactory commandFactory, IRemember cache)
            : base(configuration, commandFactory)
        {
            _cache = cache;
        }

        public CachedDocumentRepository(string configurationName, IRemember cache)
            : base(configurationName)
        {
            _cache = cache;
        }
    }

    public class CachedDocumentRepository<TModel> 
        : BaseDocumentRepository<TModel, Guid, string>, IDocumentRepository<TModel>
        where TModel : class, ICouchDocument<Guid, string>
    {
        protected IRemember _cache;

        public CachedDocumentRepository(IDocumentRepository<Guid, string> repository, IRemember cache)
            : base(repository)
        {
            _cache = cache;
        }

        public CachedDocumentRepository(string configurationName, IRemember cache)
            : base(configurationName)
        {
            _cache = cache;
        }

        public override void Save(IEnumerable<TModel> list)
        {
            list.ForEach(x =>
                         {
                             var mode =
                                 _cache.Get<TModel>(x.Id.ToString()) != default(TModel)
                                     ? StoreMode.Replace
                                     : StoreMode.Set;
                             _cache.Store(mode, x.Id.ToString(), x);
                         });
            base.Save(list);
        }

        public override void Save(TModel model)
        {
            var mode =
                _cache.Get<TModel>(model.Id.ToString()) != default(TModel)
                    ? StoreMode.Replace
                    : StoreMode.Set;
            _cache.Store(mode, model.Id.ToString(), model);
            base.Save(model);
        }

        public override IList<TModel> GetAll(int pageSize, int pageNumber)
        {
            var allKey = string.Format("{0}{1}_{2}_{3}",
                                       typeof(TModel).FullName,
                                       "_fullList",
                                       pageSize,
                                       pageNumber);

            var list = _cache.Get(allKey) as IList<TModel>;
            if (list == null)
            {
                list = base.GetAll(pageSize, pageNumber);
                _cache.Store(StoreMode.Set, allKey, list);
            }
            return list;
        }

        public override IList<TModel> GetAll()
        {
            var allKey = string.Format("{0}{1}",
                                       typeof(TModel).FullName,
                                       "_fullList");

            var list = _cache.Get(allKey) as IList<TModel>;
            if (list == null)
            {
                list = base.GetAll();
                _cache.Store(StoreMode.Set, allKey, list);
            }
            return list;
        }

        public override TModel Get(Guid id)
        {
            var item = _cache.Get(id.ToString()) as TModel;
            if (item == null)
            {
                item = base.Get(id);
                _cache.Store(StoreMode.Set, id.ToString(), item);
            }
            return item;
        }
    }

    public class CachedDocumentRepository<TKey, TRev>
        : BaseDocumentRepository<TKey, TRev>
    {
        protected IRemember _cache;

        public CachedDocumentRepository(ICouchConfiguration configuration, ICouchCommandFactory commandFactory, IRemember cache)
            : base(configuration, commandFactory)
        {
            _cache = cache;
        }

        public CachedDocumentRepository(string configurationName, IRemember cache)
            : base(configurationName)
        {
            _cache = cache;
        }

        public override void DeleteDocument<TModel>(TKey id)
        {
            _cache.Remove(id.ToString());
            base.DeleteDocument<TModel>(id);
        }

        public override void DeleteDocument<TModel>(TKey id, TRev rev)
        {
            var key = "{0}_{1}".AsFormat(id.ToString(), rev.ToString());
            _cache.Remove(key);
            base.DeleteDocument<TModel>(id);
        }

        public override void Save<TModel>(IEnumerable<TModel> list)
        {
            list.ForEach(x =>
            {
                var mode =
                    _cache.Get<TModel>(x.Id.ToString()) != default(TModel)
                        ? StoreMode.Replace
                        : StoreMode.Set;
                _cache.Store(mode, x.Id.ToString(), x);
            });
            base.Save(list);
        }

        public override void Save<TModel>(TModel model)
        {
            var mode =
                _cache.Get<TModel>(model.Id.ToString()) != default(TModel)
                    ? StoreMode.Replace
                    : StoreMode.Set;
            _cache.Store(mode, model.Id.ToString(), model);
            base.Save(model);
        }

        public override IList<TModel> GetAll<TModel>(int pageSize, int pageNumber)
        {
            var allKey = string.Format("{0}{1}_{2}_{3}",
                                       typeof(TModel).FullName,
                                       "_fullList",
                                       pageSize,
                                       pageNumber);

            var list = _cache.Get(allKey) as IList<TModel>;
            if (list == null)
            {
                list = base.GetAll<TModel>(pageSize, pageNumber);
                _cache.Store(StoreMode.Set, allKey, list);
            }
            return list;
        }

        public override IList<TModel> GetAll<TModel>()
        {
            var allKey = string.Format("{0}{1}",
                                       typeof(TModel).FullName,
                                       "_fullList");

            var list = _cache.Get(allKey) as IList<TModel>;
            if (list == null)
            {
                list = base.GetAll<TModel>();
                _cache.Store(StoreMode.Set, allKey, list);
            }
            return list;
        }

        public override TModel Get<TModel>(TKey id)
        {
            var item = _cache.Get(id.ToString()) as TModel;
            if (item == null)
            {
                item = base.Get<TModel>(id);
                var store = _cache.Store(StoreMode.Set, id.ToString(), item);
            }
            return item;
        }

        public override TModel Get<TModel>(TKey id, TRev revision)
        {
            var key = "{0}_{1}_{2}".AsFormat(id.ToString(), revision.ToString());

            var item = _cache.Get(key) as TModel;
            if (item == null)
            {
                item = base.Get<TModel>(id);
                var store = _cache.Store(StoreMode.Set, key, item);
            }
            return item;
        }

        
    }

    public class CachedDocumentRepository<TModel, TKey, TRev>
        : BaseDocumentRepository<TModel, TKey, TRev>
        where TModel : class, ICouchDocument<TKey, TRev>
    {
        protected IRemember _cache;
        protected ICacheKeyBuilder _keyBuilder;

        public override void DeleteDocument(TKey id)
        {
            _cache.Remove(id.ToString());
            base.DeleteDocument(id);
        }

        public override void DeleteDocument(TKey id, TRev rev)
        {
            var key = "{0}_{1}".AsFormat(id.ToString(), rev.ToString());
            _cache.Remove(key);
            base.DeleteDocument(id);
        }

        public override void Save(IEnumerable<TModel> list)
        {
            list.ForEach(x =>
            {
                var mode =
                    _cache.Get<TModel>(x.Id.ToString()) != default(TModel)
                        ? StoreMode.Replace
                        : StoreMode.Set;
                _cache.Store(mode, x.Id.ToString(), x);
            });
            base.Save(list);
        }

        public override void Save(TModel model)
        {
            var mode =
                _cache.Get<TModel>(model.Id.ToString()) != default(TModel)
                    ? StoreMode.Replace
                    : StoreMode.Set;
            _cache.Store(mode, model.Id.ToString(), model);
            base.Save(model);
        }

        public override IList<TModel> GetAll(int pageSize, int pageNumber)
        {
            var allKey = string.Format("{0}{1}_{2}_{3}",
                                       typeof(TModel).FullName,
                                       "_fullList",
                                       pageSize,
                                       pageNumber);

            var list = _cache.Get(allKey) as IList<TModel>;
            if (list == null)
            {
                list = base.GetAll(pageSize, pageNumber);
                _cache.Store(StoreMode.Set, allKey, list);
            }
            return list;
        }

        public override IList<TModel> GetAll()
        {
            var allKey = string.Format("{0}{1}",
                                       typeof(TModel).FullName,
                                       "_fullList");

            var list = _cache.Get(allKey) as IList<TModel>;
            if (list == null)
            {
                list = base.GetAll();
                _cache.Store(StoreMode.Set, allKey, list);
            }
            return list;
        }

        public override TModel Get(TKey id)
        {
            var item = _cache.Get(id.ToString()) as TModel;
            if (item == null)
            {
                item = base.Get(id);
                var store = _cache.Store(StoreMode.Set, id.ToString(), item);
            }
            return item;
        }

        public override TModel Get(TKey id, TRev revision)
        {
            var key = "{0}_{1}_{2}".AsFormat(id.ToString(), revision.ToString());

            var item = _cache.Get(key) as TModel;
            if (item == null)
            {
                item = base.Get(id);
                var store = _cache.Store(StoreMode.Set, key, item);
            }
            return item;
        }
        
        public CachedDocumentRepository(IDocumentRepository<TKey, TRev> repository, IRemember cache, ICacheKeyBuilder keyBuilder) : base(repository)
        {
            _cache = cache;
            _keyBuilder = keyBuilder;
        }

        public CachedDocumentRepository(string configurationName, IRemember cache, ICacheKeyBuilder keyBuilder)
            : base(configurationName)
        {
            _cache = cache;
            _keyBuilder = keyBuilder;
        }
    }

    public interface ICacheKeyBuilder
    {
        string GetKey<TModel>(object id);
        string GetKey<TModel>(object id, object rev);
        string GetListKey<TModel>();
        string GetListKey<TModel>(int page, int size);
    }

    public class CacheKeyBuilder : ICacheKeyBuilder
    {
        public string GetKey<TModel>(object id)
        {
            return "{0}_{1}"
                .AsFormat(typeof(TModel).FullName, id);
        }

        public string GetKey<TModel>(object id, object rev)
        {
            return "{0}_{1}_{2}"
                .AsFormat(typeof(TModel).FullName, id, rev);
        }

        public string GetListKey<TModel>()
        {
            return "{0}_list"
                .AsFormat(typeof(TModel).FullName);
        }

        public string GetListKey<TModel>(int page, int size)
        {
            return "{0}_{1}_{2}"
                .AsFormat(typeof(TModel).FullName, page, size);
        }
    }
}