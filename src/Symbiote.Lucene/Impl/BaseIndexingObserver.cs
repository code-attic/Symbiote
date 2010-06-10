using System;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace Symbiote.Lucene.Impl
{
    public abstract class BaseIndexingObserver : ILuceneIndexer
    {
        protected IndexWriter indexWriter { get; set; }
        protected Document document { get; set; }

        public abstract void OnNext(Tuple<string, string> value);

        public abstract void OnError(Exception error);

        public abstract void OnCompleted();

        protected BaseIndexingObserver(IndexWriter indexWriter)
        {
            this.indexWriter = indexWriter;
        }
    }
}