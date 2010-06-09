using System;
using System.Collections.Generic;

namespace Symbiote.Lucene
{
    public interface ILuceneConfiguration
    {
        string DefaultIndexPath { get; set; }
        IDictionary<string, IDirectoryFactory> DirectoryFactories { get; set; }
        IDictionary<string, IAnalyzerFactory> AnalyzerFactories { get; set; }
        IDictionary<string, string> DirectoryPaths { get; set; }
    }
}