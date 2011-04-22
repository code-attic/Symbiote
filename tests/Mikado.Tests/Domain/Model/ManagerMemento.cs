using System;
using Symbiote.Core;

namespace Mikado.Tests.Domain.Model
{
    public class ManagerMemento : IMemento<Manager>
    {
        public void Capture(Manager instance)
        {
            FirstName = instance.FirstName;
            LastName = instance.LastName;
            Age = instance.Age;
            Id = instance.Id;
            Ssn = instance.Ssn;
            Department = instance.Department;
            AddressMemento = new AddressMemento();
            AddressMemento.Capture(instance.Address);
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public Guid Id { get; set; }
        public string Ssn { get; set; }
        public string Department { get; set; }
        public AddressMemento AddressMemento { get; set; }

        public void Reset(Manager instance)
        {
            instance.Age = Age;
            instance.FirstName = FirstName;
            instance.LastName = LastName;
            instance.Id = Id;
            instance.Ssn = Ssn;
            instance.Department = Department;
            AddressMemento.Reset( instance.Address );
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
                           Address = AddressMemento.Retrieve()
                       };
        }
    }
}