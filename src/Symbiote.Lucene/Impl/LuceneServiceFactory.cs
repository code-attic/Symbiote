using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;
using StructureMap;
using StructureMap.Pipeline;
using Symbiote.Core.Extensions;
using Symbiote.Lucene.Config;

namespace Symbiote.Lucene.Impl
{
    public class LuceneServiceFactory : ILuceneServiceFactory
    {
        protected ILuceneConfiguration configuration { get; set; }
        protected ConcurrentDictionary<string, Analyzer> analyzers;
        protected ConcurrentDictionary<string, Directory> indices;
        protected ConcurrentDictionary<string, IndexWriter> indexWriters;
        
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

        public virtual Analyzer GetAnalyzer(string indexName)
        {
            Analyzer analyzer = null;
            if (!analyzers.TryGetValue(indexName, out analyzer))
            {
                IAnalyzerFactory factory = null;
                configuration.AnalyzerFactories.TryGetValue(indexName, out factory);
                factory = new DefaultAnalyzerFactory();

                analyzer = factory.CreateAnalyzerFor(indexName);
                analyzers.TryAdd(indexName, analyzer);
            }
            return analyzer;
        }

        public virtual IndexWriter GetIndexWriter(string indexName)
        {
            IndexWriter writer;
            if (!indexWriters.TryGetValue(indexName, out writer))
            {
                var index = GetIndex(indexName);
                var analyzer = GetAnalyzer(indexName);
                writer = new IndexWriter(index, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
                writer.SetRAMBufferSizeMB(512);
                writer.MaybeMerge();
                indexWriters.TryAdd(indexName, writer);
            }
            return writer;
        }

        public ILuceneIndexer GetIndexingObserverForIndex(string indexName)
        {
            var writer = GetIndexWriter(indexName);
            var args = new ExplicitArguments(new Dictionary<string, object>()
                                                 {
                                                     {"indexWriter",writer}
                                                 });
            return ObjectFactory.GetInstance<BaseIndexingObserver>(args);
        }

        public ILuceneSearchProvider GetSearchProviderForIndex(string indexName)
        {
            var writer = GetIndexWriter(indexName);
            var analyzer = GetAnalyzer(indexName);
            var args = new ExplicitArguments(new Dictionary<string, object>()
                                                 {
                                                     {"indexWriter",writer},
                                                     {"analyzer", analyzer}
                                                 });
            return ObjectFactory.GetInstance<BaseSearchProvider>(args);
        }

        public LuceneServiceFactory(ILuceneConfiguration configuration)
        {
            this.configuration = configuration;
            analyzers = new ConcurrentDictionary<string, Analyzer>();
            indices = new ConcurrentDictionary<string, Directory>();
            indexWriters = new ConcurrentDictionary<string, IndexWriter>();
        }

        public void Dispose()
        {
            indexWriters.ForEach(x => x.Value.Close(true));
            indexWriters.Clear();

            analyzers.ForEach(x => x.Value.Close());
            analyzers.Clear();

            indices.ForEach(x => x.Value.Close());
            indices.Clear();
        }
    }
}