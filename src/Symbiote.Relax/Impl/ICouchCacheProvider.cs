using System;
using System.Collections.Generic;

namespace Symbiote.Relax.Impl
{
    public interface ICouchCacheProvider
    {
        void AddCrossReference(string key, string cacheKey);

        void InvalidateItem<TModel>(string affectedKey)
            where TModel : class, ICouchDocument;

        void Delete<TModel>(object key, Action<object> delete)
            where TModel : class, ICouchDocument;

        void Delete<TModel>(object key, object rev, Action<object, object> delete)
            where TModel : class, ICouchDocument;

        void DeleteAll<TModel>()
            where TModel : class, ICouchDocument;

        TModel Get<TModel>(object key, object rev, Func<object, object, TModel> retrieve)
            where TModel : class, ICouchDocument;

        TModel Get<TModel>(object key, Func<object, TModel> retrieve)
            where TModel : class, ICouchDocument;

        IList<TModel> GetAll<TModel>(Func<IList<TModel>> retrieve)
            where TModel : class, ICouchDocument;

        IList<TModel> GetAll<TModel>(int pageNumber, int pageSize, Func<int, int, IList<TModel>> retrieve)
            where TModel : class, ICouchDocument;

        void Save<TModel>(TModel model, Action<TModel> save)
            where TModel : class, ICouchDocument;

        void Save<TModel>(IEnumerable<TModel> list, Action<IEnumerable<TModel>> save)
            where TModel : class, ICouchDocument;
    }
}