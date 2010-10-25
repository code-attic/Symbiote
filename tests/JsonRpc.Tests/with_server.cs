using Machine.Specifications;
using Symbiote.Core;
using Symbiote.JsonRpc.Host;

namespace JsonRpc.Tests
{
    public abstract class with_server : with_configuration
    {
        protected static IJsonRpcHost server;

        private Establish context = () =>
                                        {
                                            server = Assimilate.GetInstanceOf<IJsonRpcHost>();
                                            server.Start();
                                        };

        private Cleanup clean = () => server.Stop();
    }
}