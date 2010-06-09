using Lucene.Net.Analysis;
using StructureMap;

namespace Symbiote.Lucene
{
    public class AnalyzerFactory
    {
        protected ILuceneConfiguration configuration { get; set; }

        public Analyzer GetAnalyzer()
        {
            return ObjectFactory.GetInstance(configuration.AnalyzerType) as Analyzer;
        }

        public AnalyzerFactory(ILuceneConfiguration configuration)
        {
            this.configuration = configuration;
        }
    }
}