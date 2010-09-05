using System;
using System.ServiceModel;
using Machine.Specifications;
using Symbiote.Wcf.Server;

namespace Wcf.Tests
{
    public abstract class with_wcf_http_server
    {
        protected static WcfServiceHost host;
        protected static WcfServerConfigurator wcfServerConfig;

        private Establish context = () =>
                                        {
                                            wcfServerConfig = new WcfServerConfigurator(@"http://localhost:9000")
                                                .AddService<TestService>(x => x
                                                        .Timeout(TimeSpan.FromSeconds(10))
                                                        .Binding(new WSHttpBinding(SecurityMode.None)));
                                            host = new WcfServiceHost(wcfServerConfig);
                                            host.Start();
                                        };

        private Cleanup clean = () => host.Stop();
    }
}