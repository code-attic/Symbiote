using System;
using System.Collections.Generic;

namespace Symbiote.Relax
{
    public interface IDocumentRepository<TKey, TRev>
        : IDisposable
    {
        void CreateDatabase<TModel>()
            where TModel : class, ICouchDocument<TKey, TRev>;
        void DeleteDatabase<TModel>()
            where TModel : class, ICouchDocument<TKey, TRev>;
        void DeleteDocument<TModel>(TKey id, TRev rev)
            where TModel : class, ICouchDocument<TKey, TRev>;
        bool DatabaseExists<TModel>()
            where TModel : class, ICouchDocument<TKey, TRev>;
        IList<string> DatabaseList { get; }
        TModel Get<TModel>(TKey id, TRev revision)
            where TModel : class, ICouchDocument<TKey, TRev>;
        TModel Get<TModel>(TKey id)
            where TModel : class, ICouchDocument<TKey, TRev>;
        IList<TModel> GetAll<TModel>()
            where TModel : class, ICouchDocument<TKey, TRev>;
        IList<TModel> GetAll<TModel>(int pageSize, int pageNumber)
            where TModel : class, ICouchDocument<TKey, TRev>;
        void Save<TModel>(TModel model)
            where TModel : class, ICouchDocument<TKey, TRev>;
        void Save<TModel>(IEnumerable<TModel> list)
            where TModel : class, ICouchDocument<TKey, TRev>;
        void HandleUpdates<TModel>(int since, Action<ChangeRecord> onUpdate, AsyncCallback updatesInterrupted)
            where TModel : class, ICouchDocument<TKey, TRev>;
        void StopChangeStreaming<TModel>();
    }

    public interface IDocumentRepository<TModel, TKey, TRev>
        : IDisposable
        where TModel : class, ICouchDocument<TKey, TRev>
    {
        void CreateDatabase();
        void DeleteDatabase();
        bool DatabaseExists();
        void DeleteDocument(TKey id, TRev rev);
        IList<string> DatabaseList { get; }
        TModel Get(TKey id, TRev revision);
        TModel Get(TKey id);
        IList<TModel> GetAll();
        IList<TModel> GetAll(int pageSize, int pageNumber);
        void Save(TModel model);
        void Save(IEnumerable<TModel> list);
        void HandleUpdates(int since, Action<ChangeRecord> onUpdate, AsyncCallback updatesInterrupted);
        void StopChangeStreaming();
    }

    public interface IDocumentRepository : IDocumentRepository<Guid, string>
    {

    }

    public interface IDocumentRepository<TModel> 
        : IDocumentRepository<TModel, Guid, string>
        where TModel : class, ICouchDocument<Guid, string>
    {
        
    }
        
}