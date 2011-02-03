using System;
using System.Collections.Generic;
using System.Linq;
using Symbiote.Core;

namespace Core.Tests.Domain.Model
{
    public interface IHaveTestKey
    {
        Guid Id { get; set; }
    }

    public class DriverKeyAccessor : IKeyAccessor<IHaveTestKey>
    {
        public string GetId( IHaveTestKey actor )
        {
            return actor.Id.ToString();
        }

        public void SetId<TKey>( IHaveTestKey actor, TKey id )
        {
            actor.Id = Guid.Parse( id.ToString() );
        }
    }

    public class Driver : IHaveTestKey
    {
        public string SSN { get; protected set; }
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }
        public DateTime DateOfBirth { get; protected set; }
        public bool Valid { get; set; }
        public Address CurrentAddress { get; protected set; }
        public IList<Address> FormerAddresses { get; protected set; }
        public IList<Vehicle> Vehicles { get; protected set; }
        public Guid Id { get; set; }

        public void PopulateMemento(IDriverMemento memento)
        {
            memento.SSN = SSN;
            memento.FirstName = FirstName;
            memento.LastName = LastName;
            memento.DateOfBirth = DateOfBirth;

            memento.CurrentAddress = memento.CreateAddressMemento();
            CurrentAddress.Populate( memento.CurrentAddress );

            memento.FormerAddresses = FormerAddresses.Select( x =>
            {
                var addressMemento = memento.CreateAddressMemento();
                x.Populate( addressMemento );
                return addressMemento;
            } ).ToList();

            memento.Vehicles = Vehicles.Select( x =>
            {
                var vehicleMemento = memento.CreateVehicleMemento();
                x.Populate( vehicleMemento );
                return vehicleMemento;
            } ).ToList();
        }

        public void PopulateFromMemento(IDriverMemento memento)
        {
            SSN = memento.SSN;
            FirstName = memento.FirstName;
            LastName = memento.LastName;
            DateOfBirth = memento.DateOfBirth;

            CurrentAddress = new Address( memento.CurrentAddress );
            FormerAddresses = memento.FormerAddresses.Select( x => new Address( x ) ).ToList();
            Vehicles = memento.Vehicles.Select( x => new Vehicle( x ) ).ToList();
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

        private Driver()
        {
            FormerAddresses = new List<Address>();
            Vehicles = new List<Vehicle>();
            Id = Guid.NewGuid();
        }

        public Driver( string ssn, string firstName, string lastName, DateTime dateOfBirth) : this()
        {
            SSN = ssn;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Valid = true;
        }
    }
}
