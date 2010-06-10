using System;

namespace Symbiote.Lucene
{
    public interface ILuceneServiceFactory : IDisposable
    {
        ILuceneIndexer GetIndexingObserverForIndex(string indexName);
        ILuceneSearchProvider GetSearchProviderForIndex(string indexName);
    }
}