using System;
using Actor.Tests.Domain.Model;
using Machine.Specifications;

namespace Actor.Tests.Mementos
{
    public class with_driver
        : with_assimilation
    {
        public static Driver driver { get; set; }

        private Establish context = () =>
        {
            driver = new Driver( "123-45-6789", "Mr", "Rogers", DateTime.Parse( "01/01/1600" ) );
            driver.NewAddress( 100, "old street", "oldsville", "OK", "12345" );
            driver.AddVehicle( "00-00000000000", "chevy", "death cab", 1970 );
        };
    }
}