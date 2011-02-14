using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Couch;
using Symbiote.Couch.Config;
using Symbiote.StructureMapAdapter;
using StructureMap;

namespace Couch.Tests.Assimilation
{
    [Subject("Assimilation")]
    public class when_assimilating_without_caching
    {
        private Because of = () => Assimilate.Core<StructureMapAdapter>().Couch();

        private It should_use_CouchConfiguration_for_ICouchConfiguration = 
            () => ObjectFactory
                       .Container
                       .Model
                       .DefaultTypeFor<ICouchConfiguration>()
                       .ShouldEqual(typeof(CouchConfiguration));

        private It should_use_DocumentRepository_for_IDocumentRepository =
            () => ObjectFactory
                      .Container
                      .Model
                      .HasImplementationsFor(typeof(IDocumentRepository))
                      .ShouldBeTrue();

        private It should_have_couch_server_configured =
            () => ObjectFactory
                    .Container
                    .Model
                    .HasDefaultImplementationFor<ICouchServer>();
    }
}