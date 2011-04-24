using System;
using Symbiote.Core;

namespace Mikado.Tests.Domain.Model
{
    public class PersonMemento : IMemento<Person>
    {
        public void Capture(Person instance)
        {
            FirstName = instance.FirstName;
            LastName = instance.LastName;
            Age = instance.Age;
            Id = instance.Id;
            Ssn = instance.Ssn;
            AddressMemento = new AddressMemento();
            AddressMemento.Capture(instance.Address);
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public Guid Id { get; set; }
        public string Ssn { get; set; }
        public AddressMemento AddressMemento { get; set; }

        public void Reset(Person instance)
        {
            instance.Age = Age;
            instance.FirstName = FirstName;
            instance.LastName = LastName;
            instance.Id = Id;
            instance.Ssn = Ssn;
            AddressMemento.Reset(instance.Address);
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
                           Address = AddressMemento.Retrieve()
                       };
        }
    }
}