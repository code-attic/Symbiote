using System;
using System.Collections.Generic;
using Symbiote.Core.Extensions;
using Symbiote.Eidetic;

namespace Symbiote.Relax.Impl
{
    public class CachedDocumentRepository<TModel> : BaseDocumentRepository<TModel, Guid, string>, IDocumentRepository<TModel>
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

    public class CachedDocumentRepository : 
        BaseDocumentRepository<Guid, string>, 
        IDocumentRepository,
        IDocumentRepository<Guid, string>
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

        public override TModel Get<TModel>(Guid id)
        {
            var item = _cache.Get(id.ToString()) as TModel;
            if (item == null)
            {
                item = base.Get<TModel>(id);
                var store = _cache.Store(StoreMode.Set, id.ToString(), item);
            }
            return item;
        }
    }
}