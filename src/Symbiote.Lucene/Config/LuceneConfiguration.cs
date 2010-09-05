using System.Collections.Generic;
using Symbiote.Lucene.Impl;

namespace Symbiote.Lucene.Config
{
    public class LuceneConfiguration : ILuceneConfiguration
    {
        public int MemoryBufferLimit { get; set; }
        public long WriterLockTimeout { get; set; }
        public string IndexPath { get; set; }
        public IDictionary<string, string> DirectoryPaths { get; set; }
        public IDictionary<string, IDirectoryFactory> DirectoryFactories { get; set; }
        public IDictionary<string, IAnalyzerFactory> AnalyzerFactories { get; set; }

        public LuceneConfiguration()
        {
            MemoryBufferLimit = 512;
            WriterLockTimeout = 30;
            DirectoryPaths = new Dictionary<string, string>();
            DirectoryFactories = new Dictionary<string, IDirectoryFactory>();
            AnalyzerFactories = new Dictionary<string, IAnalyzerFactory>();
            IndexPath = @"/index";
        }
    }
}
