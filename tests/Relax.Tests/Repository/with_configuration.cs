using Machine.Specifications;
using StructureMap;
using Symbiote.Relax.Impl;

namespace Relax.Tests.Repository
{
    public abstract class with_configuration
    {
        protected static ICouchConfiguration configuration;
        private Establish context = () =>
                                        {
                                            configuration = new CouchConfiguration();
                                        };
        protected static void WireUpCommandMock(ICouchCommand commandMock)
        {
            ObjectFactory.Configure(x => x.For<ICouchCommand>().Use(commandMock));
        }
    }
}