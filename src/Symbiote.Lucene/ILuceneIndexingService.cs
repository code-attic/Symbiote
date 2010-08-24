using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Index;
using Microsoft.Practices.ServiceLocation;

namespace Symbiote.Lucene
{
    public interface ILuceneIndexingService
    {
        void IndexObservable<TObservable, TEvent>(string indexName, TObservable observable)
            where TObservable : IObservable<TEvent>;
    }

    public class LuceneIndexingService
        : ILuceneIndexingService
    {
        protected ILuceneServiceFactory serviceFactory { get; set; }
        protected Action<string> writeIndexToDisk { get; set; }

        public void IndexObservable<TObservable, TEvent>(string indexName, TObservable observable)
            where TObservable : IObservable<TEvent>
        {
            var indexer = serviceFactory.GetIndexingObserverForIndex(indexName);
            observable
                .Subscribe(x => IndexEvent(indexer, x));

            observable
                .BufferWithTime(TimeSpan.FromMinutes(2))
                .Do(x => WriteIndex(indexName));
        }

        protected void IndexEvent<T>(ILuceneIndexer indexer, T newEvent)
        {
            var visitor = ServiceLocator.Current.GetInstance<IVisit<T>>();
            visitor.Subscribe(indexer);
            visitor.Accept(newEvent);
        }

        public LuceneIndexingService(ILuceneServiceFactory serviceFactory)
        {
            this.serviceFactory = serviceFactory;
            writeIndexToDisk = WriteIndex;
        }

        protected void WriteIndex(string index)
        {
            var writer = this.serviceFactory.GetIndexWriter(index);
            writer.Commit();
        }
    }
}
