using System;
using System.Collections.Generic;
using System.Linq;
using Symbiote.Core;

namespace Mikado.Tests.Domain.Model
{
    public class ManagerMemento : IMemento<Manager>
    {
        public ManagerMemento()
        {
            AddressMementos = new List<AddressMemento>();
        }

        public void Capture(Manager instance)
        {
            FirstName = instance.FirstName;
            LastName = instance.LastName;
            Age = instance.Age;
            Id = instance.Id;
            Ssn = instance.Ssn;
            Department = instance.Department;
            AddressMementos.Clear();
            instance
                .Addresses
                .ToList()
                .ForEach( a =>
                              {
                                  var addrMemento = new AddressMemento();
                                  addrMemento.Capture( a );
                                  AddressMementos.Add( addrMemento );
                              } );
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public Guid Id { get; set; }
        public string Ssn { get; set; }
        public string Department { get; set; }
        public IList<AddressMemento> AddressMementos { get; set; }

        public void Reset(Manager instance)
        {
            instance.Age = Age;
            instance.FirstName = FirstName;
            instance.LastName = LastName;
            instance.Id = Id;
            instance.Ssn = Ssn;
            instance.Department = Department;
            instance.Addresses = AddressMementos.Select(s => s.Retrieve()).ToList();
        }

        public Manager Retrieve()
        {
            return new Manager()
                       {
                           Age = Age,
                           FirstName = FirstName,
                           LastName = LastName,
                           Ssn = Ssn,
                           Id = Id,
                           Department = Department,
                           Addresses = AddressMementos.Select( s => s.Retrieve() ).ToList()
                       };
        }
    }
}