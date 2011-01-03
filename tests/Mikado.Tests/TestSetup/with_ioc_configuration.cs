using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Mikado;
using Symbiote.Mikado.Impl;
using Symbiote.StructureMap;

namespace Mikado.Tests.TestSetup
{
    public class with_ioc_configuration
    {
        private Establish context = () => Assimilate
                                            .Core<StructureMapAdapter>()
                                            .Mikado()
                                            .Dependencies(x =>
                                                                {
                                                                    x.For<IRulesIndex>().Use<DefaultRulesIndex>().AsSingleton();
                                                                    x.For<IRunRules>().Use<DefaultRulesRunner>();
                                                                });
    }
}
