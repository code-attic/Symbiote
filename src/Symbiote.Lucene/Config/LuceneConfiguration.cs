using System.Collections.Generic;
using Symbiote.Lucene.Impl;

namespace Symbiote.Lucene.Config
{
    public class LuceneConfiguration : ILuceneConfiguration
    {
        public string DefaultIndexPath { get; set; }
        public IDictionary<string, string> DirectoryPaths { get; set; }
        public IDictionary<string, IDirectoryFactory> DirectoryFactories { get; set; }
        public IDictionary<string, IAnalyzerFactory> AnalyzerFactories { get; set; }

        public LuceneConfiguration()
        {
            DirectoryPaths = new Dictionary<string, string>();
            DirectoryFactories = new Dictionary<string, IDirectoryFactory>();
            AnalyzerFactories = new Dictionary<string, IAnalyzerFactory>();
            DefaultIndexPath = @"/index";
        }
    }
}
