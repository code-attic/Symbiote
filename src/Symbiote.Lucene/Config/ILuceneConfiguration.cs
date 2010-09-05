using System.Collections.Generic;
using Symbiote.Lucene.Impl;

namespace Symbiote.Lucene.Config
{
    public interface ILuceneConfiguration
    {
        int MemoryBufferLimit { get; set; }
        long WriterLockTimeout { get; set; }
        string IndexPath { get; set; }
        IDictionary<string, IDirectoryFactory> DirectoryFactories { get; set; }
        IDictionary<string, IAnalyzerFactory> AnalyzerFactories { get; set; }
        IDictionary<string, string> DirectoryPaths { get; set; }
    }
}