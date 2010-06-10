using System;

namespace Symbiote.Lucene
{
    public interface ILuceneIndexer : IObserver<Tuple<string, string>>
    {
    }
}