using System.Diagnostics;
using System.ServiceModel;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Wcf;
using Symbiote.Wcf.Client;
using WcfClientAssimilation = Symbiote.Wcf.WcfClientAssimilation;

namespace Wcf.Tests
{
    public abstract class with_wcf_client : with_wcf_server
    {
        protected static IService<ITestService> service;
        protected static Stopwatch watch;
        
        private Establish context = () =>
                                        {
                                            WcfClientAssimilation.WcfClient(Assimilate.Core(), x => 
                                                                        x.RegisterService<ITestService>(s =>
                                                                                                            {
                                                                                                                //s.MetadataExchangeAddress = @"net.tcp://localhost:8000/TestService";
                                                                                                                //s.Endpoint = new EndpointAddress(@"net.tcp://localhost:8000/TestService");
                                                                                                                s.Endpoint = new EndpointAddress(@"http://localhost:8000/TestService");
                                                                                                                //s.Binding = new NetTcpBinding(SecurityMode.None);
                                                                                                                s.Binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                                                                                                            }));
                                            watch = Stopwatch.StartNew();
                                            service = ServiceClientFactory.GetClient<ITestService>();
                                        };
    }
}