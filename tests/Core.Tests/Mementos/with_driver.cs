using System;
using Core.Tests.Actor.Domain.Model;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.Memento;

namespace Core.Tests.Mementos
{
    public class with_driver
        : with_assimilation
    {
        public static Driver driver { get; set; }
        public static IMemoizer Memoizer { get; set; }

        private Establish context = () =>
        {
            driver = new Driver( "123-45-6789", "Mr", "Rogers", DateTime.Parse( "01/01/1600" ) );
            Memoizer = Assimilate.GetInstanceOf<IMemoizer>();
        };
    }
}