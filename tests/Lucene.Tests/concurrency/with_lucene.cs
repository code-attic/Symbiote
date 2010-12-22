using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Lucene;
using Symbiote.StructureMap;

namespace Lucene.Tests.concurrency
{
    public abstract class with_lucene : with_index_feeder
    {
        protected static ILuceneServiceFactory factory;
        protected static ILuceneIndexer indexer;
        protected static ILuceneSearchProvider searchProvider;

        private Establish context = () =>
                                        {
                                            Assimilate.Core<StructureMapAdapter>().Lucene();
                                            factory = Assimilate.GetInstanceOf<ILuceneServiceFactory>();
                                            indexer = factory.GetIndexingObserverForIndex("default");
                                            searchProvider = factory.GetSearchProviderForIndex("default");
                                            feeder.Subscribe(indexer);
                                            feeder.Accept(dictionary);
                                        };
    }
}