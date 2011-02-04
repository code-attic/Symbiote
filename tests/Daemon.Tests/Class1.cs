using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Daemon;
using Symbiote.Daemon.BootStrap;
using Symbiote.StructureMap;

namespace Daemon.Tests
{
    public class with_assimilation
    {
        private Establish context = () =>
                                        {
                                            Assimilate
                                                .Core<StructureMapAdapter>()
                                                .Daemon( x => x.Arguments( new string[] {} ) );
                                        };
    }

    public class when_checking_for_minion_key_accessor
        : with_assimilation
    {
        public static IKeyAccessor<Minion> MinionKeyAccessor { get; set; }

        private Because of = () =>
                                 {
                                     MinionKeyAccessor = Assimilate.GetInstanceOf<IKeyAccessor<Minion>>();
                                 };

        private It should_have_accessor = () => MinionKeyAccessor.ShouldNotBeNull();
    }

}
