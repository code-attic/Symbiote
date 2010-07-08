using Machine.Specifications;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Restfully;
using Symbiote.Restfully.Impl.Rpc;

namespace Restfully.Tests
{
    public abstract class with_client : with_server
    {
        protected static RemoteProxy<ITestService> proxy;
        
        private Establish context = () =>
                                        {
                                            proxy = ServiceLocator.Current.GetInstance<RemoteProxy<ITestService>>();
                                        };
    }
}