using Machine.Specifications;
using Symbiote.Relax;

namespace Relax.Tests.Configuration
{
    public abstract class with_couch_configurator
    {
        protected static CouchConfigurator configurator;
        private Establish context = () => configurator = new CouchConfigurator();
    }
}