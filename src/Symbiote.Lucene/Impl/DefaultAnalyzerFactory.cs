using System;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;
using Version = Lucene.Net.Util.Version;

namespace Symbiote.Lucene.Impl
{
    public class DefaultAnalyzerFactory : IAnalyzerFactory
    {
        public Analyzer GetQueryAnalyzerFor(string indexName)
        {
            return new KeywordAnalyzer();
        }

        public Analyzer GetIndexAnalyzerFor(string indexName)
        {
            return new StandardAnalyzer(Version.LUCENE_29);
        }
    }
}