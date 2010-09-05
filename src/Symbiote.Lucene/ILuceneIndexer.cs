using System;
using Lucene.Net.Index;
using Symbiote.Lucene.Impl;

namespace Symbiote.Lucene
{
    public interface ILuceneIndexer : IObserver<Tuple<string, string>>
    {
        IDocumentQueue DocumentQueue { get; set; }
    }
}