using Machine.Specifications;
using Microsoft.Practices.ServiceLocation;
using Symbiote.JsonRpc.Host;

namespace JsonRpc.Tests
{
    public abstract class with_server : with_configuration
    {
        protected static IJsonRpcHost server;

        private Establish context = () =>
                                        {
                                            server = ServiceLocator.Current.GetInstance<IJsonRpcHost>();
                                            server.Start();
                                        };

        private Cleanup clean = () => server.Stop();
    }
}