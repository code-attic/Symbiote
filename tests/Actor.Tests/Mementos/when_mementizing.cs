using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Actor;
using Symbiote.Actor.Impl.Memento;
using Symbiote.Core;

namespace Actor.Tests.Mementos
{
    public interface IAddressMemento
    {
        int StreetNumber { get; set; }
        string StreetName { get; set; }
        string City { get; set; }
        string State { get; set; }
        string Zip { get; set; }
    }

    public interface IVehicleMemento
    {
        string Vin { get; set; }
        string Make { get; set; }
        string Model { get; set; }
        int Year { get; set; }
    }

    public interface IDriverMemento
    {
        string SSN { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        DateTime DateOfBirth { get; set; }
        IAddressMemento CurrentAddress { get; set; }
        IList<IAddressMemento> FormerAddresses { get; set; }
        IList<IVehicleMemento> Vehicles { get; set; }
    }

    [Serializable]
    public class Driver
    {
        public string SSN { get; protected set; }
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }
        public DateTime DateOfBirth { get; protected set; }

        public Address CurrentAddress { get; protected set; }
        public IList<Address> FormerAddresses { get; protected set; }
        public IList<Vehicle> Vehicles { get; protected set; }

        public void PopulateMemento(IDriverMemento memento)
        {
            
        }

        public void PopulateFromMemento(IDriverMemento memento)
        {
            
        }

        public void ChangeName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public void NewAddress(int streetNumber, string streetName, string city, string state, string zip)
        {
            if(CurrentAddress != null)
            {
                FormerAddresses.Add( CurrentAddress );
            }

            CurrentAddress = new Address(streetNumber, streetName, city, state, zip);
        }

        public void AddVehicle(string vin, string make, string model, int year)
        {
            Vehicles.Add( new Vehicle(vin, make, model, year)  );
        }

        public Driver()
        {
            FormerAddresses = new List<Address>();
            Vehicles = new List<Vehicle>();
        }

        public Driver( string ssn, string firstName, string lastName, DateTime dateOfBirth) : this()
        {
            SSN = ssn;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
        }
    }

    [Serializable]
    public class Address
    {
        public int StreetNumber { get; protected set; }
        public string StreetName { get; protected set; }
        public string City { get; protected set; }
        public string State { get; protected set; }
        public string Zip { get; protected set; }

        public Address() {}

        public Address( int streetNumber, string streetName, string city, string state, string zip )
        {
            StreetNumber = streetNumber;
            StreetName = streetName;
            City = city;
            State = state;
            Zip = zip;
        }
    }

    [Serializable]
    public class Vehicle
    {
        public string VIN { get; protected set; }
        public string Make { get; protected set; }
        public string Model { get; protected set; }
        public int Year { get; protected set; }

        public Vehicle() {}

        public Vehicle( string vin, string make, string model, int year )
        {
            VIN = vin;
            Make = make;
            Model = model;
            Year = year;
        }
    }

    public class DriverMemento 
        : IMemento<Driver>, IDriverMemento
    {
        public IActorFactory<Driver> Factory { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public IAddressMemento CurrentAddress { get; set; }
        public IList<IAddressMemento> FormerAddresses { get; set; }
        public IList<IVehicleMemento> Vehicles { get; set; }

        public void Capture( Driver instance )
        {
            instance.PopulateMemento( this );
        }

        public void Reset( Driver instance )
        {
            instance.PopulateFromMemento( this );
        }

        public Driver Retrieve()
        {
            var driver = Factory.CreateInstance(  )
        }

        public void WriteValues()
        {
            
        }

        public DriverMemento( IActorFactory<Driver> factory )
        {
            Factory = factory;
            FormerAddresses = new List<IAddressMemento>();
            Vehicles = new List<IVehicleMemento>();
        }
    }

    public class DriverKeyAccessor : IKeyAccessor<Driver>
    {
        public string GetId( Driver actor )
        {
            return actor.SSN;
        }

        public void SetId<TKey>( Driver actor, TKey id )
        {
            
        }
    }

    public class DriverFactory : IActorFactory<Driver>
    {
        public Driver CreateInstance<TKey>( TKey id )
        {
            
        }
    }



    public class with_driver
        : with_assimilation
    {
        public static Driver driver { get; set; }

        private Establish context = () =>
        {
            driver = new Driver( "Mr", "Rogers", DateTime.Parse( "01/01/1600" ) );
            driver.NewAddress( 100, "old street", "oldsville", "OK", "12345" );
            driver.AddVehicle( "00-00000000000", "chevy", "death cab", 1970 );
        };
    }

    public class when_mementizing_via_reflection
        : with_driver
    {
        protected static Driver clone { get; set; }
        protected static Driver reset { get; set; }

        private Because of = () =>
        {
            var memoizer = Assimilate.GetInstanceOf<IMemoizer>();
            var memento = memoizer.GetMemento( driver );
            clone = memento.Retrieve();
            reset = memento.Retrieve();
            reset.ChangeName( "Kaptain", "Kangaroo" );
            memento.Reset( reset );
        };

        private It should_produce_different_instance_from_original = () => ReferenceEquals( driver, clone ).ShouldBeFalse();
        private It should_reset_correctly = () => (reset.FirstName == "Mr" && reset.LastName == "Rogers").ShouldBeTrue();
    }
}
