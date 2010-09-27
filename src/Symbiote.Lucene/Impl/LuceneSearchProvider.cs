/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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