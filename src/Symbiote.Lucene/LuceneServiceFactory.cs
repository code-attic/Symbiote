using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using StructureMap;

namespace Symbiote.Lucene
{
    public class LuceneServiceFactory
    {
        protected ILuceneConfiguration configuration { get; set; }
        public Directory Directory { get; protected set; }
        public Analyzer Analyzer { get; protected set; }
        public IndexWriter IndexWriter { get; protected set; }
        public IndexSearcher IndexSearcher { get; protected set; }

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
            return new LuceneSearchProvider(this.IndexSearcher, this.Analyzer);
        }

        public LuceneServiceFactory(ILuceneConfiguration configuration)
        {
            this.configuration = configuration;
            Directory = GetDirectory();
            Analyzer = GetAnalyzer();
            IndexWriter = new IndexWriter(Directory, Analyzer, true);
            IndexSearcher = new IndexSearcher(Directory);
        }
    }
}