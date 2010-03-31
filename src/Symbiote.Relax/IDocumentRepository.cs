using System;
using System.Collections.Generic;

namespace Symbiote.Relax
{
    public interface IDocumentRepository<TModel>
        : IDisposable
        where TModel : class, ICouchDocument
    {
        void CreateDatabase();
        void DeleteDatabase();
        bool DatabaseExists();
        void DeleteDocument<TKey>(TKey id);
        void DeleteDocument<TKey,TRev>(TKey id, TRev rev);
        IList<string> DatabaseList { get; }
        TModel Get<TKey, TRev>(TKey id, TRev revision);
        TModel Get<TKey>(TKey id);
        IList<TModel> GetAll();
        IList<TModel> GetAll(int pageSize, int pageNumber);
        void Save(TModel model);
        void Save(IEnumerable<TModel> list);
        void HandleUpdates(int since, Action<ChangeRecord> onUpdate, AsyncCallback updatesInterrupted);
        void StopChangeStreaming();
    }
}