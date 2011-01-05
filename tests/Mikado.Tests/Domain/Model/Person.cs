using System;
using Symbiote.Core;
using Symbiote.Core.Work;

namespace Mikado.Tests.Domain.Model
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Ssn { get; set; }
        public int Age { get; set; }
        public Guid Id { get; set; }

        public Person()
        {
            Id = new Guid();
        }
    }

    public class Manager : Person
    {
        public string Department { get; set; }
    }

    public class PersonMemento : IMemento<Person>
    {
        public void Capture(Person instance)
        {
            FirstName = instance.FirstName;
            LastName = instance.LastName;
            Age = instance.Age;
            Id = instance.Id;
            Ssn = instance.Ssn;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public Guid Id { get; set; }
        public string Ssn { get; set; }

        public void Reset(Person instance)
        {
            instance.Age = Age;
            instance.FirstName = FirstName;
            instance.LastName = LastName;
            instance.Id = Id;
            instance.Ssn = Ssn;
        }

        public Person Retrieve()
        {
            return new Person()
                       {
                           Age = Age,
                           FirstName = FirstName,
                           LastName = LastName,
                           Ssn = Ssn,
                           Id = Id
                       };
        }
    }

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
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public Guid Id { get; set; }
        public string Ssn { get; set; }
        public string Department { get; set; }

        public void Reset(Manager instance)
        {
            instance.Age = Age;
            instance.FirstName = FirstName;
            instance.LastName = LastName;
            instance.Id = Id;
            instance.Ssn = Ssn;
            instance.Department = Department;
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
                Department = Department
            };
        }
    }
}
