using Machine.Specifications;
using Symbiote.Core;
using Symbiote.StructureMapAdapter;

namespace Core.Tests.DI
{
    public abstract class with_assembly_scanning_for_marker_interface
    {
        private Establish context = () =>
                                        {
                                            Assimilate
                                                .Core<StructureMapAdapter>()
                                                .Dependencies(x => x.Scan(s =>
                                                                              {
                                                                                  s.AssemblyContainingType<IAmAnInterface>();
                                                                                  s.AddAllTypesOf<AnInterfaceOf>();
                                                                              }));
                                        };

    }
}