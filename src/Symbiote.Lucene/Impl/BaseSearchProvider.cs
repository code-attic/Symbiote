using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace Symbiote.Lucene.Impl
{
    public abstract class BaseSearchProvider : ILuceneSearchProvider
    {
        protected IndexWriter indexWriter { get; set; }
        protected Searcher indexSearcher
        {
            get
            {
                return new IndexSearcher(indexWriter.GetDirectory(), true);
            }
        }
        protected Analyzer analyzer { get; set; }

        public abstract IEnumerable<Tuple<ScoreDoc, Document>> GetDocumentsForQuery(string field, string queryText);

        protected BaseSearchProvider(IndexWriter indexWriter, Analyzer analyzer)
        {
            this.indexWriter = indexWriter;
            this.analyzer = analyzer;
        }
    }
}