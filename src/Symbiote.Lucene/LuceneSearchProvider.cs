using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;

namespace Symbiote.Lucene
{
    public interface ILuceneSearchProvider
    {
        IEnumerable<Tuple<ScoreDoc, Document>> GetDocumentsForQuery(string field, string queryText);
    }

    public abstract class BaseSearchProvider : ILuceneSearchProvider
    {
        protected IndexWriter indexWriter { get; set; }
        protected IndexSearcher indexSearcher
        {
            get { return new IndexSearcher(indexWriter.GetReader()); }
        }
        protected Analyzer analyzer { get; set; }

        public abstract IEnumerable<Tuple<ScoreDoc, Document>> GetDocumentsForQuery(string field, string queryText);

        protected BaseSearchProvider(IndexWriter indexWriter, Analyzer analyzer)
        {
            this.indexWriter = indexWriter;
            this.analyzer = analyzer;
        }
    }

    public class LuceneSearchProvider : BaseSearchProvider
    {
        public override IEnumerable<Tuple<ScoreDoc, Document>> GetDocumentsForQuery(string field, string queryText)
        {
            var parser = new QueryParser(field, analyzer);
            var query = parser.Parse(queryText);
            var collector = TopScoreDocCollector.create(1000, true);
            indexSearcher.Search(query, collector);
            var hits = collector.TopDocs().scoreDocs;
            for (int i = 0; i < hits.Length; i++)
            {
                var scoreDoc = hits[i];
                yield return Tuple.Create(scoreDoc, indexSearcher.Doc(scoreDoc.doc));
            }
            yield break;
        }

        public LuceneSearchProvider(IndexWriter indexWriter, Analyzer analyzer) : base(indexWriter, analyzer)
        {
        }
    }
}