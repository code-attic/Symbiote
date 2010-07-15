using System;
using Lucene.Net.Index;

namespace Symbiote.Lucene
{
    public interface ILuceneIndexer : IObserver<Tuple<string, string>>
    {
        IndexWriter IndexWriter { get; set; }
    }
}