using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Version = Lucene.Net.Util.Version;

namespace Symbiote.Lucene.Impl
{
    public class LuceneSearchProvider : BaseSearchProvider
    {
        public override IEnumerable<Tuple<ScoreDoc, Document>> GetDocumentsForQuery(string queryText)
        {
            var parser = new CustomQueryParser(Version.LUCENE_29, "", Analyzer);
            parser.SetAllowLeadingWildcard(true);
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
    }
}