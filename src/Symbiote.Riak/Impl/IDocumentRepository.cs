using Symbiote.Riak.Impl.Data;

namespace Symbiote.Riak.Impl
{
    public interface IDocumentRepository
    {
        Document<T> GetDocument<T>(string bucket, string key, uint minimum);
        void PersistDocument<T>(string bucket, string key, string vectorClock, Document<T> document, uint write, uint dw, bool returnBody);
    }
}