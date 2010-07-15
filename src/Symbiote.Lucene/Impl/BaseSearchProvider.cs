using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Symbiote.Core;

namespace Symbiote.Lucene.Impl
{
    public abstract class BaseSearchProvider : ILuceneSearchProvider
    {
        protected Searcher indexSearcher
        {
            get
            {
                var dirSearcher = new IndexSearcher(IndexWriter.GetDirectory(), true);
                var readerSearcher = new IndexSearcher(IndexWriter.GetReader());
                var searcher = new MultiSearcher(new Searchable[] {dirSearcher, readerSearcher});
                return searcher;
            }
        }

        public IndexWriter IndexWriter { get; set; }
        public Analyzer Analyzer { get; set; }

        public abstract IEnumerable<Tuple<ScoreDoc, Document>> GetDocumentsForQuery(string queryText);
        
        public virtual IEnumerable<Tuple<ScoreDoc, Document>> GetDocumentsForQuery<TModel>(Expression<Func<TModel, bool>> predicate)
        {
            DelimitedBuilder builder = new DelimitedBuilder("");
            ExpressionTreeProcessor.Process(predicate, "", builder);
            var query = builder.ToString();
            return GetDocumentsForQuery(query);
        }
    }
}