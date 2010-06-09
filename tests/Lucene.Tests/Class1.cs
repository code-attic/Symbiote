using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Machine.Specifications;
using StructureMap;
using Symbiote.Core;
using Symbiote.Lucene;
using Symbiote.Core.Extensions;

namespace Lucene.Tests
{
    public class IndexFeeder
        : IVisit<Dictionary<string,string>>
    {
        protected ConcurrentBag<IObserver<Tuple<string, string>>> observers { get; set; }

        public IDisposable Subscribe(IObserver<Tuple<string, string>> observer)
        {
            var disposable = observer as IDisposable;
            observers.Add(observer);
            return disposable;
        }

        public void Accept(Dictionary<string, string> root)
        {
            root
                .ForEach(x => observers
                                  .ForEach(y => y.OnNext(Tuple.Create(x.Key, x.Value))));

            observers
                .ForEach(y => y.OnCompleted());
        }

        public IndexFeeder()
        {
            observers = new ConcurrentBag<IObserver<Tuple<string, string>>>();
        }
    }

    public abstract class with_index_feeder
    {
        protected static Dictionary<string, string> dictionary;
        protected static IndexFeeder feeder;

        private Establish context = () =>
                                        {
                                            feeder = new IndexFeeder();
                                            dictionary = new Dictionary<string, string>()
                                                             {
                                                                 {"FirstName", "Alex"},
                                                                 {"LastName", "Robson"},
                                                                 {"Age", "31"},
                                                             };
                                        };
    }

    public abstract class with_lucene : with_index_feeder
    {
        protected static LuceneServiceFactory factory;
        protected static ILuceneIndexer indexer;
        protected static ILuceneSearchProvider searchProvider;

        private Establish context = () =>
                                        {
                                            Assimilate.Core().Lucene(x => x.UseDefaults());
                                            factory = ObjectFactory.GetInstance<LuceneServiceFactory>();
                                            indexer = factory.GetIndexingObserverForIndex("default");
                                            searchProvider = factory.GetSearchProviderForIndex("default");
                                            feeder.Subscribe(indexer);
                                            feeder.Accept(dictionary);
                                        };
    }

    public class when_searching : with_lucene
    {
        protected static List<Tuple<ScoreDoc, Document>> documents;
        protected static Stopwatch watch;
        protected static int age;
        
        private Because of = () =>
                                 {
                                     watch = Stopwatch.StartNew();
                                     documents = searchProvider.GetDocumentsForQuery("FirstName", "Alex").ToList();
                                     watch.Stop();
                                     age = int.Parse(documents[0].Item2.Get("Age"));
                                 };
        
        private It should_return_one_document = () => 
            documents.Count.ShouldEqual(1);

        private It should_have_age_31 = () => age.ShouldEqual(31);
    }
}
