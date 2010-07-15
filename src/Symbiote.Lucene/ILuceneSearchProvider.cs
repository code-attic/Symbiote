using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace Symbiote.Lucene
{
    public interface ILuceneSearchProvider
    {
        IndexWriter IndexWriter { get; set; }
        Analyzer Analyzer { get; set; }

        IEnumerable<Tuple<ScoreDoc, Document>> GetDocumentsForQuery(string queryText);
        IEnumerable<Tuple<ScoreDoc, Document>> GetDocumentsForQuery<TModel>(Expression<Func<TModel,  bool>> predicate);
    }
}