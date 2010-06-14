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
        protected IndexWriter indexWriter { get; set; }
        protected Searcher indexSearcher
        {
            get
            {
                return new IndexSearcher(indexWriter.GetDirectory(), true);
            }
        }
        protected Analyzer analyzer { get; set; }

        public abstract IEnumerable<Tuple<ScoreDoc, Document>> GetDocumentsForQuery(string queryText);
        
        public virtual IEnumerable<Tuple<ScoreDoc, Document>> GetDocumentsForQuery<TModel>(Expression<Func<TModel, bool>> predicate)
        {
            DelimitedBuilder builder = new DelimitedBuilder("");
            ExpressionTreeProcessor.Process(predicate, "", builder);
            var query = builder.ToString();
            return GetDocumentsForQuery(query);
        }

        protected BaseSearchProvider(IndexWriter indexWriter, Analyzer analyzer)
        {
            this.indexWriter = indexWriter;
            this.analyzer = analyzer;
        }
    }
}