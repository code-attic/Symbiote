using System;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Store;

namespace Symbiote.Lucene
{
    public class LuceneConfiguration : ILuceneConfiguration
    {
        public Type DirectoryType { get; set; }
        public Type AnalyzerType { get; set; }

        public LuceneConfiguration()
        {
            DirectoryType = typeof(RAMDirectory);
            AnalyzerType = typeof(StandardAnalyzer);
        }
    }
}
