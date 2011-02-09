using System.Diagnostics;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.StructureMapAdapter;
using Symbiote.Wcf;
using Symbiote.Wcf.Client;
using WcfClientAssimilation = Symbiote.Wcf.WcfClientAssimilation;

namespace Wcf.Tests
{
    public abstract class with_wcf_nettcp_client_from_mex : with_wcf_nettcp_server
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
                                                                    s.MetadataExchangeAddress = @"http://localhost:8001/TestService";
                                                                }));
                                            watch = Stopwatch.StartNew();
                                            service = ServiceClientFactory.GetClient<ITestService>();
                                        };
    }
}