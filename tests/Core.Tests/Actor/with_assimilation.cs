using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Messaging;
using Symbiote.StructureMap;

namespace Actor.Tests
{
    public class with_assimilation
    {
        public static IBus Bus { get; set; }

        private Establish context = () =>
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Messaging();
            Bus = Assimilate.GetInstanceOf<IBus>();
        };
    }
}