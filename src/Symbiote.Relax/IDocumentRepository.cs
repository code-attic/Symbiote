using System;
using System.Collections.Generic;

namespace Symbiote.Relax
{
    public interface IDocumentRepository
        : IDisposable
        
    {
        void CreateDatabase<TModel>()
            where TModel : class, ICouchDocument;
        void DeleteDatabase<TModel>()
            where TModel : class, ICouchDocument;
        bool DatabaseExists<TModel>()
            where TModel : class, ICouchDocument;
        void DeleteDocument<TModel>(object id) 
            where TModel : class, ICouchDocument;
        void DeleteDocument<TModel>(object id, object rev)
            where TModel : class, ICouchDocument;
        IList<string> DatabaseList { get; }
        TModel Get<TModel>(object id, object revision)
            where TModel : class, ICouchDocument;
        TModel Get<TModel>(object id)
            where TModel : class, ICouchDocument;
        IList<TModel> GetAll<TModel>()
            where TModel : class, ICouchDocument;
        IList<TModel> GetAll<TModel>(int pageSize, int pageNumber)
            where TModel : class, ICouchDocument;
        void Save<TModel>(TModel model)
            where TModel : class, ICouchDocument;
        void SaveAll<TModel>(IEnumerable<TModel> list)
            where TModel : class, ICouchDocument;
        void HandleUpdates<TModel>(int since, Action<ChangeRecord> onUpdate, AsyncCallback updatesInterrupted)
            where TModel : class, ICouchDocument;
        void StopChangeStreaming<TModel>()
            where TModel : class, ICouchDocument;
    }
}