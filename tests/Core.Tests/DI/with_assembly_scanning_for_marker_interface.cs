using Machine.Specifications;
using Symbiote.Core;

namespace Core.Tests.DI
{
    public abstract class with_assembly_scanning_for_marker_interface
    {
        private Establish context = () => Assimilate
                                              .Initialize()
                                              .Dependencies(x => x.Scan(s => s.AddAllTypesOf<AnInterfaceOf>() ));

    }
}