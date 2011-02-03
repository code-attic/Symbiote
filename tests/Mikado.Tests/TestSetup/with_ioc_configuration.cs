using Machine.Specifications;
using Mikado.Tests.Domain.Model;
using Symbiote.Core;
using Symbiote.Core.UnitOfWork;
using Symbiote.Mikado;
using Symbiote.Mikado.Impl;
using Symbiote.StructureMap;

namespace Mikado.Tests.TestSetup
{
    public class with_ioc_configuration
    {
        private Establish context = () => Assimilate
                                            .Core<StructureMapAdapter>()
                                            .Mikado();
    }
}
