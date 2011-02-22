using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Lucene;
using Symbiote.Lucene.Impl;
using Symbiote.StructureMapAdapter;

namespace Lucene.Tests
{
    public abstract class with_lucene : with_index_feeder
    {
        protected static ILuceneServiceFactory factory;
        protected static ILuceneIndexer indexer;
        protected static ILuceneSearchProvider searchProvider;

        private Establish context = () =>
                                        {
                                            Assimilate.Initialize();
                                            factory = Assimilate.GetInstanceOf<ILuceneServiceFactory>();
                                            indexer = factory.GetIndexingObserverForIndex("default");
                                            searchProvider = factory.GetSearchProviderForIndex("default");
                                            feeder.Subscribe(indexer);
                                            feeder.Accept(dictionary);
                                        };
    }
}