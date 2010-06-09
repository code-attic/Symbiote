using System;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace Symbiote.Lucene
{
    public interface ILuceneIndexer : IObserver<Tuple<string, string>>
    {
    }

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

    public class LuceneIndexingObserver
        : BaseIndexingObserver
    {
        public override void OnNext(Tuple<string, string> value)
        {
            document.Add(new Field(value.Item1, value.Item2, Field.Store.YES, Field.Index.ANALYZED));
        }

        public override void OnError(Exception error)
        {

        }

        public override void OnCompleted()
        {
            indexWriter.AddDocument(document);
            indexWriter.Optimize();
            indexWriter.Commit();
        }

        public LuceneIndexingObserver(IndexWriter indexWriter)
            : base(indexWriter)
        {
            this.indexWriter = indexWriter;
            document = new Document();
        }
    }
}