using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Wcf.Client;
using Symbiote.Wcf.Server;
using Symbiote.Wcf;

namespace Wcf.Tests
{
    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        bool TwoArgCall(DateTime date, Guid id);
    }

    public class TestService : ITestService
    {
        public bool TwoArgCall(DateTime date, Guid id)
        {
            return true;
        }

        public TestService()
        {
        }
    }

    public abstract class with_wcf_server
    {
        protected static WcfServiceHost host;
        protected static WcfServerConfigurator wcfServerConfig;

        private Establish context = () =>
                                        {
                                            wcfServerConfig = new WcfServerConfigurator(@"http://localhost:8000")
                                                .AddService<TestService>(x => x.UseDefaults());
                                            host = new WcfServiceHost(wcfServerConfig);
                                            host.Start();
                                        };
    }

    public abstract class with_wcf_client : with_wcf_server
    {
        protected static IService<ITestService> service;

        private Establish context = () =>
                                        {
                                            Assimilate.Core().WcfClient(x => x.RegisterService<ITestService>(s => s.MetadataExchangeAddress = @"http://localhost:8000/TestService"));
                                            service = ServiceClientFactory.GetClient<ITestService>();
                                        };
    }

    public class when_calling_wcf_service_via_dynamic_proxy : with_wcf_client
    {
        protected static Stopwatch watch;
        protected static bool result;

        private Because of = () =>
                                 {
                                     watch = Stopwatch.StartNew();
                                     var dateTime = DateTime.Now;
                                     var newGuid = Guid.NewGuid();
                                     result = service.Call(x => x.TwoArgCall(dateTime, newGuid));
                                     watch.Stop();
                                     host.Stop();
                                 };
        
        private It should_run_in_time = () => watch.ElapsedMilliseconds.ShouldBeLessThan(100);
        private It should_return_true = () => result.ShouldBeTrue();
    }
}
