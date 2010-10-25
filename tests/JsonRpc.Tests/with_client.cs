using Machine.Specifications;
using Symbiote.Core;
using Symbiote.JsonRpc.Client;

namespace JsonRpc.Tests
{
    public abstract class with_client : with_server
    {
        protected static IRemoteProxy<ITestService> proxy;
        
        private Establish context = () =>
                                        {
                                            proxy = Assimilate.GetInstanceOf<IRemoteProxy<ITestService>>();
                                        };
    }
}