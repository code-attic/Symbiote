using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Messaging;
using Symbiote.StructureMap;

namespace Actor.Tests
{
    public class with_assimilation
    {
        private Establish context = () =>
                                        {
                                            Assimilate
                                                .Core<StructureMapAdapter>()
                                                .Messaging();
                                            Bus = Assimilate.GetInstanceOf<IBus>();
                                        };

        public static IBus Bus { get; set; }
    }
}