using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.StructureMap;
using Symbiote.Actor;
using Symbiote.Messaging;

namespace Domain.Tests
{
    public class with_assimilation
    {
        private Establish context = () =>
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Actors()
                .Messaging( x => x.PublishEventsTo( "local" ) );
        };
    }

    public class when_creating_auction
    {

    }
}
