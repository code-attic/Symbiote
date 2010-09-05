using System.Collections.Concurrent;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using ObservableExtensions = System.ObservableExtensions;
using System;

namespace Symbiote.Lucene.Impl
{
    public class DocumentQueue 
        : IDocumentQueue
    {
        public IndexWriter IndexWriter { get; set; }
        public ConcurrentQueue<Document> Documents { get; set; }
        public object Lock { get; set; }

        public void PushDocument(Document document)
        {
            Documents.Enqueue(document);
        }

        public void IndexDocument(Document document)
        {
            lock(Lock)
            {
                this.IndexWriter.AddDocument(document);
            }
        }

        public DocumentQueue(IndexWriter indexWriter)
        {
            Lock = new object();
            IndexWriter = indexWriter;
            Documents = new ConcurrentQueue<Document>();
            Documents.ToObservable().Subscribe(x => IndexDocument(x));
        }
    }
}