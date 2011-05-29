using Machine.Specifications;
using Messaging.Tests;
using Symbiote.Core;
using Symbiote.Messaging;

namespace Actor.Tests.Sagas
{
    public class with_bus
        : with_assimilation
    {
        public static IBus Bus { get; set; }
        private Establish context = () =>
                                        {
                                            Bus = Assimilate.GetInstanceOf<IBus>();
                                            Bus.AddLocalChannel(x => x.CorrelateBy<SetPersonName>( m => m.PersonId.ToString() ));
                                        };
    }
}