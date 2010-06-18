using Lucene.Net.Analysis;

namespace Symbiote.Lucene.Impl
{
    public interface IAnalyzerFactory
    {
        Analyzer GetQueryAnalyzerFor(string indexName);
        Analyzer GetIndexAnalyzerFor(string indexName);
    }
}