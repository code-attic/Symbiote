using System.ServiceModel;
using Machine.Specifications;
using Symbiote.Wcf.Server;

namespace Wcf.Tests
{
    public abstract class with_wcf_server
    {
        protected static WcfServiceHost host;
        protected static WcfServerConfigurator wcfServerConfig;

        private Establish context = () =>
                                        {
                                            wcfServerConfig = new WcfServerConfigurator(@"http://localhost:8000")
                                                .AddService<TestService>(x =>
                                                                             {
                                                                                 x.UseDefaults();
                                                                                 //x.GetConfiguration().Binding =
                                                                                 //    new NetTcpBinding(SecurityMode.None);
                                                                                 
                                                                                 x.GetConfiguration().Binding =
                                                                                     new BasicHttpBinding(BasicHttpSecurityMode.None);
                                                                             });
                                            host = new WcfServiceHost(wcfServerConfig);
                                            host.Start();
                                        };
    }
}