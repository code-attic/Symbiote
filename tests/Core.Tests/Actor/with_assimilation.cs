using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Messaging;
using Symbiote.StructureMapAdapter;

namespace Actor.Tests
{
    public class with_assimilation
    {
        private Establish context = () =>
                                        {
                                            Assimilate
                                                .Initialize();
                                            Bus = Assimilate.GetInstanceOf<IBus>();
                                        };

        public static IBus Bus { get; set; }
    }
}