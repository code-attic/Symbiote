using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Actor.Tests.Mementos
{
    public class Driver
    {
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }
        public DateTime DateOfBirth { get; protected set; }

        public Address CurrentAddress { get; protected set; }
        public IList<Address> FormerAddresses { get; protected set; }
        public IList<Vehicle> Vehicles { get; protected set; }

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

        public Driver( string firstName, string lastName, DateTime dateOfBirth) : this()
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
        }
    }

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
        : with_assimilation
    {

    }
}
