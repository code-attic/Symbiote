using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.UnitOfWork;
using Symbiote.StructureMapAdapter;
using Symbiote.Messaging;
using Symbiote.Rabbit;

namespace Rabbit.Tests
{
    public class with_rabbit_configuration
    {
        protected static IBus Bus { get; set; }
        private Establish context = () =>
                                        {
                                            Assimilate
                                                .Initialize()
                                                .Rabbit(x => x.AddBroker(r => r.Defaults().Address("localhost")));
                                            Bus = Assimilate.GetInstanceOf<IBus>();
                                        };
    }
}
