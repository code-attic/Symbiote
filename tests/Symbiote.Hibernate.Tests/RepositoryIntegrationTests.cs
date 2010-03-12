using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Mapping;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Symbiote.TestSpec;

namespace Symbiote.Hibernate.Tests
{
    public class RepositoryIntegrationTests : Spec
    {
        public ISessionFactory _factory;
        public ISessionManager manager;
        public Repository<Person> repository;

        public override void Initialize()
        {
            base.Initialize();
            HibernatingRhinos.NHibernate.Profiler.Appender.NHibernateProfiler.Initialize();
            _factory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008.ConnectionString(cs => cs.Server("localhost").Database("sliver").TrustedConnection()))
                .Mappings(m =>
                {
                    m.FluentMappings.AddFromAssemblyOf<Person>();
                })
                .ExposeConfiguration(c =>
                {
                    var schema = new SchemaExport(c);
                    schema.Drop(false, true);
                    schema.Execute(false, true, false);
                })
                .BuildSessionFactory();

            var context = new InMemoryContext();
            manager = new SessionManager(context);
            manager.CurrentSession = _factory.OpenSession();
            repository = new Repository<Person>(manager);
        }

        public override void Arrange()
        {
            
        }

        public override void Act()
        {
            
        }

        [Assert]
        public void AShouldInsertRecords()
        {
            var peoples = new[]
                          {
                              new Person() {DateOfBirth = DateTime.Now, Name = "Alex", Vehicles =
                                                                                           {
                                                                                               new Vehicle() { Make = "Chevy", Model="Equinox", Year=2007},
                                                                                           }},
                              new Person() {DateOfBirth = DateTime.Now, Name = "Rebekah", Vehicles = { new Vehicle() { Make="Honda", Model="Civic", Year=2008}}},
                              new Person() {DateOfBirth = DateTime.Now, Name = "Dexter", Vehicles = { new Vehicle() {Make = "Tonka", Model="Dumptruck", Year=2010}}},
                          };

            repository.Insert<Person>(peoples);
            repository.Commit();
            repository.GetCount(Select.All<Person>()).ShouldEqual(3);
        }

        [Assert]
        public void ShouldGetByCriteria()
        {
            repository.FindOne(Select.Where<Person>(x => x.Name == "Alex"))
                .Name.ShouldEqual("Alex");
        }

        public override void Finish()
        {
            manager.CurrentSession.Close();
            manager.CurrentSession.Dispose();
        }
    }

    public class Person
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime DateOfBirth { get; set; }
        public virtual IList<Vehicle> Vehicles { get; set; }

        public Person()
        {
            Vehicles = new List<Vehicle>();
        }
    }

    public class Vehicle
    {
        public virtual long Id { get; set; }
        public virtual Person Owner { get; set; }
        public virtual string Make { get; set; }
        public virtual string Model { get; set; }
        public virtual int Year { get; set; }
    }

    public class PersonMap : ClassMap<Person>
    {
        public PersonMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Name);
            Map(x => x.DateOfBirth);
            HasMany(x => x.Vehicles)
                .AsBag()
                .Cascade.All()
                .Fetch.Select();
        }
    }

    public class VehicleMap : ClassMap<Vehicle>
    {
        public VehicleMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Make);
            Map(x => x.Model);
            Map(x => x.Year);
            References(x => x.Owner);
        }
    }

    public class PersonFetchingStrategy : IFetchingStrategy<Person>.For<Person>
    {
        public IQueryable<Person> ApplyTo(IQueryable<Person> queryable)
        {
            return queryable;
        }
    }
}
