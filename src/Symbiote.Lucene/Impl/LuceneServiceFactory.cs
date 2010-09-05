using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core.Extensions;
using Symbiote.Lucene.Config;

namespace Symbiote.Lucene.Impl
{
    public class LuceneServiceFactory : ILuceneServiceFactory
    {
        protected ILuceneConfiguration configuration { get; set; }
        protected ConcurrentDictionary<string, Analyzer> indexingAnalyzers;
        protected ConcurrentDictionary<string, Analyzer> queryAnalyzers;
        protected ConcurrentDictionary<string, Directory> indices;
        protected ConcurrentDictionary<string, IndexWriter> indexWriters;
        protected ConcurrentDictionary<string, IDocumentQueue> documentQueues;
        
        public virtual Directory GetIndex(string indexName)
        {
            Directory directory = null;
            if(!indices.TryGetValue(indexName, out directory))
            {
                IDirectoryFactory factory = null;
                configuration.DirectoryFactories.TryGetValue(indexName, out factory);
                factory = factory ?? new DefaultDirectoryFactory(configuration);

                directory = factory.CreateDirectoryFor(indexName);
                indices.TryAdd(indexName, directory);
            }
            return directory;
        }

        public virtual Analyzer GetIndexingAnalyzer(string indexName)
        {
            Analyzer analyzer = null;
            if (!indexingAnalyzers.TryGetValue(indexName, out analyzer))
            {
                IAnalyzerFactory factory = null;
                configuration.AnalyzerFactories.TryGetValue(indexName, out factory);
                factory = new DefaultAnalyzerFactory();

                analyzer = factory.GetIndexAnalyzerFor(indexName);
                indexingAnalyzers.TryAdd(indexName, analyzer);
            }
            return analyzer;
        }

        public virtual Analyzer GetQueryAnalyzer(string indexName)
        {
            Analyzer analyzer = null;
            if (!queryAnalyzers.TryGetValue(indexName, out analyzer))
            {
                IAnalyzerFactory factory = null;
                configuration.AnalyzerFactories.TryGetValue(indexName, out factory);
                factory = new DefaultAnalyzerFactory();

                analyzer = factory.GetQueryAnalyzerFor(indexName);
                queryAnalyzers.TryAdd(indexName, analyzer);
            }
            return analyzer;
        }

        public virtual IndexWriter GetIndexWriter(string indexName)
        {
            IndexWriter writer;
            if (!indexWriters.TryGetValue(indexName, out writer))
            {
                var index = GetIndex(indexName);
                var analyzer = GetIndexingAnalyzer(indexName);
                writer = new IndexWriter(index, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
                writer.SetRAMBufferSizeMB(configuration.MemoryBufferLimit);
                writer.SetWriteLockTimeout(configuration.WriterLockTimeout);
                writer.MaybeMerge();
                indexWriters.TryAdd(indexName, writer);
            }
            return writer;
        }

        public virtual IDocumentQueue GetDocumentQueue(string indexName)
        {
            IDocumentQueue documentQueue;
            if (!documentQueues.TryGetValue(indexName, out documentQueue))
            {
                var writer = GetIndexWriter(indexName);
                documentQueue = new DocumentQueue(writer);
                documentQueues.TryAdd(indexName, documentQueue);
            }
            return documentQueue;
        }

        public ILuceneIndexer GetIndexingObserverForIndex(string indexName)
        {
            var indexer = ServiceLocator.Current.GetInstance<BaseIndexingObserver>();
            indexer.DocumentQueue = GetDocumentQueue(indexName);
            return indexer;
        }

        public ILuceneSearchProvider GetSearchProviderForIndex(string indexName)
        {
            var searchProvider = ServiceLocator.Current.GetInstance<BaseSearchProvider>();
            searchProvider.IndexWriter = GetIndexWriter(indexName);
            searchProvider.Analyzer = GetQueryAnalyzer(indexName);
            return searchProvider;
        }

        public LuceneServiceFactory(ILuceneConfiguration configuration)
        {
            this.configuration = configuration;
            queryAnalyzers = new ConcurrentDictionary<string, Analyzer>();
            indexingAnalyzers = new ConcurrentDictionary<string, Analyzer>();
            indices = new ConcurrentDictionary<string, Directory>();
            indexWriters = new ConcurrentDictionary<string, IndexWriter>();
            documentQueues = new ConcurrentDictionary<string, IDocumentQueue>();
        }

        public void Dispose()
        {
            indexWriters.ForEach(x => x.Value.Close(true));
            indexWriters.Clear();

            queryAnalyzers.ForEach(x => x.Value.Close());
            queryAnalyzers.Clear();

            indexingAnalyzers.ForEach(x => x.Value.Close());
            indexingAnalyzers.Clear();

            indices.ForEach(x => x.Value.Close());
            indices.Clear();

            documentQueues.Clear();
        }
    }
}