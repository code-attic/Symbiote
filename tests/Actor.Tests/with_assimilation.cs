using Machine.Specifications;
using Symbiote.Actor;
using Symbiote.Core;
using Symbiote.StructureMap;

namespace Actor.Tests
{
    public class with_assimilation
    {
        private Establish context = () =>
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Actors();
        };
    }
}