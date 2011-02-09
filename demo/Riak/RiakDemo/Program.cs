using System.Linq;
using System.Runtime.Serialization;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Messaging;
using Symbiote.StructureMapAdapter;
using Symbiote.Log4Net;
using Symbiote.Daemon;
using Symbiote.Riak;

namespace RiakDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .AddConsoleLogger<RiakService>( x => x.Info().MessageLayout( m => m.Message().Newline() ) )
                .Messaging()
                .Daemon( x => x.Arguments( args ).Name( "Riak Demo" ) )
                .Riak( x => x.AddNode( r => r.Address( "192.168.1.100" ) ) )
                .RunDaemon();
        }
    }

    [DataContract]
    public class Person
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }
        
        [DataMember(Order = 2)]
        public string FirstName { get; set; }
        
        [DataMember(Order = 3)]
        public string LastName { get; set; }

        public Person() {}

        public Person( int id, string firstName, string lastName )
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
        }
    }

    public class PersonKeyAccessor
        : IKeyAccessor<Person>
    {
        public string GetId( Person actor )
        {
            return actor.Id.ToString();
        }

        public void SetId<TKey>( Person actor, TKey id )
        {
            actor.Id = int.Parse( id.ToString() );
        }
    }

    public class RiakService : IDaemon
    {
        public IKeyValueStore KeyValues { get; set; }
        public IRepository Repository { get; set; }

        public void Start()
        {
            "Starting Key Value Store Demo"
                .ToInfo<RiakService>();

            DemoKeyValueStore();

            "Starting Repository Demo"
                .ToInfo<RiakService>();

            DemoRepository();
        }

        private void DemoKeyValueStore()
        {
            var newPerson = new Person( 1, "First", "Person" );

            "Saving new person: {0} - {1} {2}".ToInfo<RiakService>(newPerson.Id, newPerson.FirstName, newPerson.LastName);
            KeyValues.Persist( newPerson.Id.ToString(), newPerson );

            "Retrieving new person".ToInfo<RiakService>();
            var retrieved = KeyValues.Get<Person>( "1" );

            "Retrieved: {0} - {1} {2}".ToInfo<RiakService>(retrieved.Id, retrieved.FirstName, retrieved.LastName);

            var additional = new Person( 2, "Second", "Person" );
            "Adding another person: {0} - {1} {2}".ToInfo<RiakService>(additional.Id, additional.FirstName, additional.LastName);

            "Retrieving all people".ToInfo<RiakService>();
            var people = KeyValues.GetAll<Person>();

            "Retrieved {0} people.".ToInfo<RiakService>(people.Count());
            
            "Deleting people".ToInfo<RiakService>();
            KeyValues.Delete<Person>( "1" );
            KeyValues.Delete<Person>( "2" );
        }

        private void DemoRepository()
        {
            var people = new Person[]
            {
                new Person(1, "First", "Person"),     
                new Person(2, "Second", "Person"),     
                new Person(3, "Third", "Person"),     
                new Person(4, "Fourth", "Person"),     
                new Person(5, "Fifth", "Person"),
            };

            "Saving all 5 people".ToInfo<RiakService>();
            var success = people.All( Repository.Persist );

            "People were persisted {0}".ToInfo<RiakService>( success ? "successfully" : "unsuccessully" );

            "Retrieving all people".ToInfo<RiakService>();
            var retrieved = Repository.GetAll<Person>();

            "Retrieved {0} people".ToInfo<RiakService>( retrieved.Count() );

            "Deleting all the people".ToInfo<RiakService>();
            success = people.All( Repository.Delete );

            "People were persisted {0}".ToInfo<RiakService>(success ? "successfully" : "unsuccessully");
        }

        public void Stop()
        {
            
        }

        public RiakService( IKeyValueStore keyValues, IRepository repository )
        {
            KeyValues = keyValues;
            Repository = repository;
        }
    }
}
