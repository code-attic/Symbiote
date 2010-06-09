using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Store;

namespace Symbiote.Lucene
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
