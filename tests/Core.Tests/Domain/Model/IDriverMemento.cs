using System;
using System.Collections.Generic;
using Core.Tests.Domain.Memoization;

namespace Core.Tests.Domain.Model
{
    public interface IDriverMemento
    {
        string SSN { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        DateTime DateOfBirth { get; set; }
        IAddressMemento CurrentAddress { get; set; }
        IList<AddressMemento> FormerAddresses { get; set; }
        IList<VehicleMemento> Vehicles { get; set; }

        IAddressMemento CreateAddressMemento();
        IVehicleMemento CreateVehicleMemento();
    }
}