using System;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace Symbiote.Lucene
{
    public class LuceneIndexingObserver
        : IObserver<Tuple<string, string>>
    {
        protected IndexWriter indexWriter { get; set; }
        protected Document document { get; set; }

        public void OnNext(Tuple<string, string> value)
        {
            document.Add(new Field(value.Item1, value.Item2, Field.Store.YES, Field.Index.TOKENIZED));
        }

        public void OnError(Exception error)
        {

        }

        public void OnCompleted()
        {
            indexWriter.AddDocument(document);
            indexWriter.Optimize();
        }

        public LuceneIndexingObserver(IndexWriter writer)
        {
            indexWriter = writer;
            document = new Document();
        }
    }
}