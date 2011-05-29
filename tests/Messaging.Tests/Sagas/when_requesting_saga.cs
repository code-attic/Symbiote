using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using Messaging.Tests;
using Symbiote.Core;
using Symbiote.Messaging.Impl.Saga;

namespace Actor.Tests.Sagas
{
    public class when_requesting_saga
        : with_assimilation
    {
        public static IEnumerable<ISaga> Sagas { get; set; }

        private Because of = () =>
        {
            Sagas = Assimilate.GetAllInstancesOf<ISaga>();
        };

        private It should_not_be_empty = () => 
            Sagas.Count().ShouldBeGreaterThan( 0 );
    }
}
