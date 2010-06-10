using System;
using System.Collections.Generic;
using Lucene.Net.Documents;
using Lucene.Net.Search;

namespace Symbiote.Lucene
{
    public interface ILuceneSearchProvider
    {
        IEnumerable<Tuple<ScoreDoc, Document>> GetDocumentsForQuery(string field, string queryText);
    }
}