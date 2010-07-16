using Machine.Specifications;
using Moq;
using Symbiote.Core;
using Symbiote.JsonRpc;
using Symbiote.StructureMap;
using Assimilate = Symbiote.Core.Assimilate;

namespace Restfully.Tests
{
    public abstract class with_configuration
    {
        protected static Mock<ITestService> serviceMock;
        private Establish context = () =>
                                        {
                                            serviceMock = new Mock<ITestService>();
                                            Assimilate
                                                .Core<StructureMapAdapter>()
                                                .HttpServiceHost(x => x.UseDefaults().HostService<ITestService>())
                                                .HttpServiceClient(x => x.Server(@"http://localhost:8420/").Timeout(9000))
                                                .Dependencies(x => x.For<ITestService>().Use(serviceMock.Object));
                                        };
    }
}