using Machine.Specifications;
using Symbiote.Core;
using Symbiote.StructureMap;

namespace Core.Tests
{
    public class with_assimilation
    {
        private Establish context = () =>
        {
            Assimilate
                .Core<StructureMapAdapter>();
        };
    }
}