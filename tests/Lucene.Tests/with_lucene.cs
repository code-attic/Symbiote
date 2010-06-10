using Machine.Specifications;
using StructureMap;
using Symbiote.Core;
using Symbiote.Lucene;

namespace Lucene.Tests
{
    public abstract class with_lucene : with_index_feeder
    {
        protected static ILuceneServiceFactory factory;
        protected static ILuceneIndexer indexer;
        protected static ILuceneSearchProvider searchProvider;

        private Establish context = () =>
                                        {
                                            LuceneAssimilation.Lucene(Assimilate.Core(), x => x.UseDefaults());
                                            factory = ObjectFactory.GetInstance<ILuceneServiceFactory>();
                                            indexer = factory.GetIndexingObserverForIndex("default");
                                            searchProvider = factory.GetSearchProviderForIndex("default");
                                            feeder.Subscribe(indexer);
                                            feeder.Accept(dictionary);
                                        };
    }
}