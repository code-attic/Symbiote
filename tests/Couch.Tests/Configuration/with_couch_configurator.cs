using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Couch.Config;

namespace Couch.Tests.Configuration
{
    public abstract class with_couch_configurator
    {
        protected static CouchConfigurator configurator;
        private Establish context = () =>
                                        {
                                            configurator = new CouchConfigurator();
                                            Assimilate.Initialize();
                                        };
    }
}