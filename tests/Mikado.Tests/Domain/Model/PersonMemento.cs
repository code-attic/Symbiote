using System;
using System.Collections.Generic;
using System.Linq;
using Symbiote.Core;

namespace Mikado.Tests.Domain.Model
{
    public class PersonMemento : IMemento<Person>
    {
        public PersonMemento()
        {
            AddressMementos = new List<AddressMemento>();
        }

        public void Capture(Person instance)
        {
            FirstName = instance.FirstName;
            LastName = instance.LastName;
            Age = instance.Age;
            Id = instance.Id;
            Ssn = instance.Ssn;
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
        public IList<AddressMemento> AddressMementos { get; set; }

        public void Reset(Person instance)
        {
            instance.Age = Age;
            instance.FirstName = FirstName;
            instance.LastName = LastName;
            instance.Id = Id;
            instance.Ssn = Ssn;
            instance.Addresses = AddressMementos.Select( s => s.Retrieve() ).ToList();
        }

        public Person Retrieve()
        {
            return new Person()
                       {
                           Age = Age,
                           FirstName = FirstName,
                           LastName = LastName,
                           Ssn = Ssn,
                           Id = Id,
                           Addresses = AddressMementos.Select( s => s.Retrieve() ).ToList()
                       };
        }
    }
}