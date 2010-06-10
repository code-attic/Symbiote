using Machine.Specifications;
using StructureMap;
using Symbiote.Core;
using Symbiote.Lucene;

namespace Lucene.Tests
{
    public abstract class with_index_on_disk
    {
        protected static ILuceneServiceFactory factory;
        protected static ILuceneSearchProvider searchProvider;

        private Establish context = () =>
                                        {
                                            LuceneAssimilation.Lucene(Assimilate.Core(), x => x.UseDefaults());
                                            factory = ObjectFactory.GetInstance<ILuceneServiceFactory>();
                                            searchProvider = factory.GetSearchProviderForIndex("default");
                                        };
    }
}