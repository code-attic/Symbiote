using Machine.Specifications;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core;
using Symbiote.Lucene;
using Symbiote.Lucene.Impl;
using Symbiote.StructureMap;

namespace Lucene.Tests
{
    public abstract class with_lucene : with_index_feeder
    {
        protected static ILuceneServiceFactory factory;
        protected static ILuceneIndexer indexer;
        protected static ILuceneSearchProvider searchProvider;

        private Establish context = () =>
                                        {
                                            LuceneAssimilation.Lucene(Assimilate.Core<StructureMapAdapter>(), x => x.UseDefaults());
                                            factory = ServiceLocator.Current.GetInstance<ILuceneServiceFactory>();
                                            indexer = factory.GetIndexingObserverForIndex("default");
                                            searchProvider = factory.GetSearchProviderForIndex("default");
                                            feeder.Subscribe(indexer);
                                            feeder.Accept(dictionary);
                                        };
    }
}