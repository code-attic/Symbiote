using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lucene.Net.Documents;
using Lucene.Net.Search;

namespace Symbiote.Lucene
{
    public interface ILuceneSearchProvider
    {
        IEnumerable<Tuple<ScoreDoc, Document>> GetDocumentsForQuery(string queryText);
        IEnumerable<Tuple<ScoreDoc, Document>> GetDocumentsForQuery<TModel>(Expression<Func<TModel,  bool>> predicate);
    }
}