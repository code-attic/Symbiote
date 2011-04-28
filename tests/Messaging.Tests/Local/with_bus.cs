using Machine.Specifications;
using Messaging.Tests.Local.HandleInterface;
using Symbiote.Core;
using Symbiote.Messaging;

namespace Messaging.Tests.Local
{
    public class with_bus
        : with_assimilation
    {
        protected static IBus bus { get; set; }

        private Establish context = () =>
                                        {
                                            bus = Assimilate.GetInstanceOf<IBus>();
                                            bus.AddLocalChannel( );
                                        };
    }
}