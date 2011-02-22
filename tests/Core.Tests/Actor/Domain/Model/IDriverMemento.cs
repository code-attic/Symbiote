using System;
using System.Collections.Generic;

namespace Core.Tests.Actor.Domain.Model
{
    public interface IDriverMemento
    {
        string SSN { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        DateTime DateOfBirth { get; set; }
        IAddressMemento CurrentAddress { get; set; }
        IList<IAddressMemento> FormerAddresses { get; set; }
        IList<IVehicleMemento> Vehicles { get; set; }

        IAddressMemento CreateAddressMemento();
        IVehicleMemento CreateVehicleMemento();
    }
}