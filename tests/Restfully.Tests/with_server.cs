using Machine.Specifications;
using StructureMap;
using Symbiote.Restfully;
using Symbiote.Restfully.Impl;

namespace Restfully.Tests
{
    public abstract class with_server : with_configuration
    {
        protected static IHttpServiceHost server;

        private Establish context = () =>
                                        {
                                            server = ObjectFactory.GetInstance<IHttpServiceHost>();
                                            server.Start();
                                        };
    }
}