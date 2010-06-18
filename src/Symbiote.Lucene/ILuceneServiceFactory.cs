using System;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;

namespace Symbiote.Lucene
{
    public interface ILuceneServiceFactory : IDisposable
    {
        ILuceneIndexer GetIndexingObserverForIndex(string indexName);
        ILuceneSearchProvider GetSearchProviderForIndex(string indexName);
        Directory GetIndex(string indexName);
        IndexWriter GetIndexWriter(string indexName);
        Analyzer GetQueryAnalyzer(string indexName);
        Analyzer GetIndexingAnalyzer(string indexName);
    }
}