using Machine.Specifications;
using Symbiote.Core;
using Symbiote.StructureMapAdapter;
using Symbiote.Couch;

namespace Couch.Tests.Commands
{
    public abstract class with_configuration
    {
        private Establish context = () =>
                                        {
                                            Assimilate.Core<StructureMapAdapter>().Couch();
                                        };
    }
}