using System.Diagnostics;
using System.ServiceModel;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.StructureMapAdapter;
using Symbiote.Wcf;
using Symbiote.Wcf.Client;
using WcfClientAssimilation = Symbiote.Wcf.WcfClientAssimilation;

namespace Wcf.Tests
{
    public abstract class with_wcf_http_client : with_wcf_http_server
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
                                                                    s.Endpoint = new EndpointAddress(@"http://localhost:9000/TestService");
                                                                    s.Binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                                                                }));
                                            watch = Stopwatch.StartNew();
                                            service = ServiceClientFactory.GetClient<ITestService>();
                                        };
    }
}