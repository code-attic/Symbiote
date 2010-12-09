using System;
using System.Collections.Generic;
using Actor.Tests.Domain.Model;
using Symbiote.Actor;

namespace Actor.Tests.Domain.Memoization
{
    public class DriverMemento 
        : IMemento<Driver>, IDriverMemento
    {
        public DriverFactory Factory { get; set; }
        public string SSN { get; set; }
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
            var driver = Factory.CreateNewDriver( SSN, FirstName, LastName, DateOfBirth );
            driver.PopulateFromMemento( this );
            return driver;
        }

        public IAddressMemento CreateAddressMemento()
        {
            return new AddressMemento();
        }

        public IVehicleMemento CreateVehicleMemento()
        {
            return new VehicleMemento();
        }

        public DriverMemento( DriverFactory factory )
        {
            Factory = factory;
            FormerAddresses = new List<IAddressMemento>();
            Vehicles = new List<IVehicleMemento>();
        }
    }
}