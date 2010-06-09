using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using StructureMap;
using StructureMap.Pipeline;

namespace Symbiote.Lucene
{
    public interface ILuceneServiceFactory
    {
        ILuceneIndexer GetIndexingObserverForIndex(string indexName);
        ILuceneSearchProvider GetSearchProviderForIndex(string indexName);
    }

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
                IDirectoryFactory factory = new DefaultDirectoryFactory(configuration);
                configuration.DirectoryFactories.TryGetValue(indexName, out factory);

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
                IAnalyzerFactory factory = new DefaultAnalyzerFactory();
                configuration.AnalyzerFactories.TryGetValue(indexName, out factory);

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
    }

    public interface IDirectoryFactory
    {
        Directory CreateDirectoryFor(string indexName);
    }

    public interface IAnalyzerFactory
    {
        Analyzer CreateAnalyzerFor(string indexName);
    }

    public class DefaultDirectoryFactory : IDirectoryFactory
    {
        protected ILuceneConfiguration configuration;

        public Directory CreateDirectoryFor(string indexName)
        {
            var indexFullPath = configuration.DirectoryPaths + @"/" + indexName;
            configuration.DirectoryPaths.TryGetValue(indexName, out indexFullPath);

            var directoryInfo = System.IO.Directory.CreateDirectory(indexFullPath);

            return new RAMDirectory(
                new SimpleFSDirectory(
                    directoryInfo,
                    new SimpleFSLockFactory()));
        }

        public DefaultDirectoryFactory (ILuceneConfiguration configuration)
        {
            this.configuration = configuration;
        }
    }

    public class DefaultAnalyzerFactory : IAnalyzerFactory
    {
        public Analyzer CreateAnalyzerFor(string indexName)
        {
            return new StandardAnalyzer();
        }
    }

    public class LuceneServiceFactory_OLD
    {
        protected ILuceneConfiguration configuration { get; set; }
        public Directory Directory { get; protected set; }
        public Analyzer Analyzer { get; protected set; }
        public IndexWriter IndexWriter { get; protected set; }
        
        protected IndexSearcher IndexSearcher
        {
            get { return new IndexSearcher(IndexWriter.GetReader());}
        }

        protected Directory GetDirectory()
        {
            return new RAMDirectory();
            //return ObjectFactory.GetInstance(configuration.DirectoryType) as Directory;
        }

        protected Analyzer GetAnalyzer()
        {
            return new StandardAnalyzer();
            //return ObjectFactory.GetInstance(configuration.AnalyzerType) as Analyzer;
        }

        public LuceneIndexingObserver CreateIndexingObserver()
        {
            return new LuceneIndexingObserver(this.IndexWriter);
        }

        public LuceneSearchProvider CreateSearchProvider()
        {
            return new LuceneSearchProvider(this.IndexWriter, this.Analyzer);
        }

        public LuceneServiceFactory_OLD(ILuceneConfiguration configuration)
        {
            this.configuration = configuration;
            Directory = GetDirectory();
            Analyzer = GetAnalyzer();
            IndexWriter = new IndexWriter(Directory, Analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);
        }
    }
}