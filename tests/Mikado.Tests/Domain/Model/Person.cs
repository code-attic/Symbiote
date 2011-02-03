using System;
using Symbiote.Core;

namespace Mikado.Tests.Domain.Model
{
    public interface ITestKey
    {
        Guid Id { get; set; }
    }

    public class TestKeyAccessor : IKeyAccessor<ITestKey>
    {
        public string GetId( ITestKey actor )
        {
            return actor.Id.ToString();
        }

        public void SetId<TKey>( ITestKey actor, TKey id )
        {
            actor.Id = Guid.Parse(id.ToString());
        }
    }

    public interface IHaveFirstName
    {
        string FirstName { get; set; }
    }

    public interface IHaveLastName
    {
        string LastName { get; set; }
    }

    public interface IHaveAge
    {
        int Age { get; set; }
    }

    public interface IHaveDepartment
    {
        string Department { get; set; }
    }

    public interface IPerson : IHaveAge, IHaveFirstName, IHaveLastName, ITestKey
    {
        
    }

    public interface IManager : IPerson, IHaveDepartment
    {
        
    }

    public class Person : IPerson
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

    public class Manager : Person, IManager
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
