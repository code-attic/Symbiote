using Machine.Specifications;
using Moq;
using Symbiote.Core;
using Symbiote.JsonRpc.Client;
using Symbiote.JsonRpc.Host;
using Symbiote.StructureMap;

namespace JsonRpc.Tests
{
    public abstract class with_configuration
    {
        protected static Mock<ITestService> serviceMock;
        private Establish context = () =>
                                        {
                                            serviceMock = new Mock<ITestService>();
                                            Assimilate
                                                .Core<StructureMapAdapter>()
                                                .JsonRpcHost(x => x.AddPort(8281).HostService<ITestService>())
                                                .JsonRpcClient(x => x.Server(@"http://localhost:8281").Timeout(300))
                                                .Dependencies(x => x.For<ITestService>().Use(serviceMock.Object));
                                        };
    }
}