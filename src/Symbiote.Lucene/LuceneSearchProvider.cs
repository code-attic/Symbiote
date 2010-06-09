using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;

namespace Symbiote.Lucene
{
    public class LuceneSearchProvider
    {
        protected IndexSearcher indexSearcher { get; set; }
        protected Analyzer analyzer { get; set; }

        public IEnumerable<Document> GetHitsForQuery(string field, string queryText)
        {
            var parser = new QueryParser(field, analyzer);
            var query = parser.Parse(queryText);
            var results = indexSearcher.Search(query);
            for (int i = 0; i < results.Length(); i++)
            {
                yield return results.Doc(i);
            }
            yield break;
        }

        public LuceneSearchProvider(IndexSearcher indexSearcher, Analyzer analyzer)
        {
            this.indexSearcher = indexSearcher;
            this.analyzer = analyzer;
        }
    }
}