using System.ServiceModel;
using Machine.Specifications;
using Symbiote.Wcf.Server;

namespace Wcf.Tests
{
    public abstract class with_wcf_nettcp_server
    {
        protected static WcfServiceHost host;
        protected static WcfServerConfigurator wcfServerConfig;

        private Establish context = () =>
                                        {
                                            wcfServerConfig = new WcfServerConfigurator(@"net.tcp://localhost:8000")
                                                .AddService<TestService>(x =>
                                                                             {
                                                                                 x.Binding(new NetTcpBinding(SecurityMode.None));
                                                                                 x.MexAddress(@"http://localhost:8001");
                                                                             });
                                            host = new WcfServiceHost(wcfServerConfig);
                                            host.Start();
                                        };

        private Cleanup clean = () => host.Stop();
    }
}