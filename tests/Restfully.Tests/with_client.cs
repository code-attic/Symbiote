using Machine.Specifications;
using Microsoft.Practices.ServiceLocation;
using Symbiote.JsonRpc.Client;
using Symbiote.JsonRpc.Client.Impl.Rpc;

namespace JsonRpc.Tests
{
    public abstract class with_client : with_server
    {
        protected static IRemoteProxy<ITestService> proxy;
        
        private Establish context = () =>
                                        {
                                            proxy = ServiceLocator.Current.GetInstance<IRemoteProxy<ITestService>>();
                                        };
    }
}