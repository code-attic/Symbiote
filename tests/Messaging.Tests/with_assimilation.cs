using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Messaging;
using Symbiote.StructureMap;
using Symbiote.Actor;

namespace Messaging.Tests
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