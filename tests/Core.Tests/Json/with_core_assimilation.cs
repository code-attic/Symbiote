using Machine.Specifications;
using Symbiote.Core;
using Symbiote.StructureMap;

namespace Core.Tests
{
    public abstract class with_core_assimilation
    {
        private Establish context = () =>
                                        {
                                            Assimilate.Core<StructureMapAdapter>();
                                        };
    }
}