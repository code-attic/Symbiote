using Machine.Specifications;
using Moq;
using Symbiote.Restfully;
using Assimilate = Symbiote.Core.Assimilate;

namespace Restfully.Tests
{
    public abstract class with_configuration
    {
        protected static Mock<ITestService> serviceMock;
        private Establish context = () =>
                                        {
                                            serviceMock = new Mock<ITestService>();
                                            Assimilate.Dependencies(Assimilate
                                                                  .Core()
                                                                  .HttpServer(x => x.UseDefaults())
                                                                  .HttpClient(x => x.Server(@"http://localhost:8420/").Timeout(9000)), x => x.For<ITestService>().Use(serviceMock.Object));
                                        };
    }
}