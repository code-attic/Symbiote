using Machine.Specifications;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Restfully;
using Symbiote.Restfully.Impl;

namespace Restfully.Tests
{
    public abstract class with_server : with_configuration
    {
        protected static IHttpServiceHost server;

        private Establish context = () =>
                                        {
                                            server = ServiceLocator.Current.GetInstance<IHttpServiceHost>();
                                            server.Start();
                                        };
    }
}