using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;

namespace Symbiote.Lucene.Impl
{
    public class DefaultAnalyzerFactory : IAnalyzerFactory
    {
        public Analyzer CreateAnalyzerFor(string indexName)
        {
            return new StandardAnalyzer(Version.LUCENE_29);
        }
    }
}