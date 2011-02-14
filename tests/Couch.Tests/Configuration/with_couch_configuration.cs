using Symbiote.Couch.Config;

namespace Couch.Tests.Configuration
{
    public abstract class with_couch_configuration : with_couch_configurator
    {
        protected static ICouchConfiguration configuration { get { return configurator.GetConfiguration(); } }
    }
}