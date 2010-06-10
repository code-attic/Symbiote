using Lucene.Net.Store;

namespace Symbiote.Lucene.Impl
{
    public interface IDirectoryFactory
    {
        Directory CreateDirectoryFor(string indexName);
    }
}