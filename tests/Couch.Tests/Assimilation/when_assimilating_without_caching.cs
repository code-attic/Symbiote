using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Couch;
using Symbiote.Couch.Config;

namespace Couch.Tests.Assimilation
{
    [Subject("Assimilation")]
    public class when_assimilating_without_caching
    {
        private Because of = () => Assimilate.Initialize();

        private It should_use_CouchConfiguration_for_ICouchConfiguration = 
            () => Assimilate
                    .Assimilation
                    .DependencyAdapter
                    .GetDefaultTypeFor<ICouchConfiguration>()
                    .ShouldEqual(typeof(CouchConfiguration));

        private It should_use_DocumentRepository_for_IDocumentRepository =
            () => Assimilate
                    .Assimilation
                    .DependencyAdapter
                    .HasPluginFor<IDocumentRepository>()
                    .ShouldBeTrue();

        private It should_have_couch_server_configured =
            () => Assimilate
                    .Assimilation
                    .DependencyAdapter
                    .HasPluginFor<ICouchServer>()
                    .ShouldBeTrue();
    }
}