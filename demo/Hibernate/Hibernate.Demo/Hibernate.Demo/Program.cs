using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Daemon;
using 

namespace Hibernate.Demo
{
    class Program
    {
        static void Main(string[] args)
        {

        }
    }

    public class PersistenceService
        : IDaemon
    {
        public void Start()
        {
            
        }

        public void Stop()
        {
            
        }
    }

    public class Person
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }

        public Person()
        {
        }

        public Person(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class PersonMap : ClassMap<Person>
    {
        
    }
}
