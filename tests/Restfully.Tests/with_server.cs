using Machine.Specifications;
using StructureMap;
using Symbiote.Restfully;

namespace Restfully.Tests
{
    public abstract class with_server : with_configuration
    {
        protected static IHttpServer server;

        private Establish context = () =>
                                        {
                                            server = ObjectFactory.GetInstance<IHttpServer>();
                                            server.Start();
                                        };
    }
}