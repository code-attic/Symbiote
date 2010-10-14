using Machine.Specifications;
using Symbiote.Messaging;
using Microsoft.Practices.ServiceLocation;

namespace Messaging.Tests.Local
{
    public class with_bus
        : with_simple_actor_cache
    {
        protected static IBus bus { get; set; }

        private Establish context = () =>
                                        {
                                            bus = ServiceLocator.Current.GetInstance<IBus>();
                                        };
    }
}