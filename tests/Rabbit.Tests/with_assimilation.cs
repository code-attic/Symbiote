using Machine.Specifications;
using Symbiote.Actor;
using Symbiote.Core;
using Symbiote.Messaging;
using Symbiote.StructureMap;

namespace Rabbit.Tests
{
    public class with_assimilation
    {
        private Establish context = () =>
                                        {
                                            Assimilate
                                                .Core<StructureMapAdapter>()
                                                .Actors()
                                                .Messaging()
                                                .UseTestLogAdapter();
                                        };    
    }
}