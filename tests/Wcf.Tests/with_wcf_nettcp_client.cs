using System.Diagnostics;
using System.ServiceModel;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.StructureMap;
using Symbiote.Wcf;
using Symbiote.Wcf.Client;
using WcfClientAssimilation = Symbiote.Wcf.WcfClientAssimilation;

namespace Wcf.Tests
{
    public abstract class with_wcf_nettcp_client : with_wcf_nettcp_server
    {
        protected static IService<ITestService> service;
        protected static Stopwatch watch;

        private Establish context = () =>
                                        {
                                            Assimilate
                                                .Core<StructureMapAdapter>()
                                                .WcfClient(x =>
                                                           x.RegisterService<ITestService>(s =>
                                                                    {
                                                                        s.Endpoint = new EndpointAddress(@"net.tcp://localhost:8000/TestService");
                                                                        s.Binding = new NetTcpBinding(SecurityMode.None);
                                                                    }));
                                            watch = Stopwatch.StartNew();
                                            service = ServiceClientFactory.GetClient<ITestService>();
                                        };
    }
}