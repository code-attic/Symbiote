using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Daemon;
using Symbiote.StructureMapAdapter;

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
}
