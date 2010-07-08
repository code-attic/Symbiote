using Machine.Specifications;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core;
using Symbiote.Lucene;
using Symbiote.StructureMap;

namespace Lucene.Tests
{
    public abstract class with_index_on_disk
    {
        protected static ILuceneServiceFactory factory;
        protected static ILuceneSearchProvider searchProvider;

        private Establish context = () =>
                                        {
                                            LuceneAssimilation.Lucene(Assimilate.Core<StructureMapAdapter>(), x => x.UseDefaults());
                                            factory = ServiceLocator.Current.GetInstance<ILuceneServiceFactory>();
                                            searchProvider = factory.GetSearchProviderForIndex("default");
                                        };
    }
}