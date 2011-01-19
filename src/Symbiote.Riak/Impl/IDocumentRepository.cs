using System.Collections.Generic;
using Symbiote.Riak.Impl.Data;

namespace Symbiote.Riak.Impl
{
    public interface IDocumentRepository
    {
        void DeleteDocument<T>( string key );
        Document<T> GetDocument<T>(string key);
        IEnumerable<Document<T>> GetAllDocuments<T>();
        void PersistDocument<T>(string key, Document<T> document);
    }
}