using Lucene.Net.Analysis;

namespace Symbiote.Lucene.Impl
{
    public interface IAnalyzerFactory
    {
        Analyzer CreateAnalyzerFor(string indexName);
    }
}