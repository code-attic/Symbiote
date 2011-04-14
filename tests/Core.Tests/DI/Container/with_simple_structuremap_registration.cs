using Machine.Specifications;
using StructureMap;

namespace Core.Tests.DI.Container
{
    public class with_simple_structuremap_registration
    {
        private Establish context = () =>
            {
                ObjectFactory.Configure( x =>
                    {
                        x.For<IHazzaMessage>().Use<MessageHazzer>();
                        x.For<IMessageProvider>().Use<MessageProvider>();
                    } );
            };
    }
}