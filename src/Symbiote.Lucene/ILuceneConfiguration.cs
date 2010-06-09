using System;

namespace Symbiote.Lucene
{
    public interface ILuceneConfiguration
    {
        Type DirectoryType { get; set; }
        Type AnalyzerType { get; set; }
    }
}