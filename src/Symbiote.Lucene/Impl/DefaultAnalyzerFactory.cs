using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;

namespace Symbiote.Lucene.Impl
{
    public class DefaultAnalyzerFactory : IAnalyzerFactory
    {
        public Analyzer CreateAnalyzerFor(string indexName)
        {
            return new StandardAnalyzer();
        }
    }
}