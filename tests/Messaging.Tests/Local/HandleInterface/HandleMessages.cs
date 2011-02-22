using System;
using System.Collections.Generic;
using Symbiote.Core;
using Symbiote.Messaging;

namespace Messaging.Tests.Local.HandleInterface
{
    public class HandleMessages : IHandle<IAmAMessage>
    {
        public static List<string> Messages = new List<string>();

        public Action<IEnvelope> Handle( IAmAMessage message )
        {
            Messages.Add( message.Text );
            return x => x.Acknowledge();
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public void ChangeName(string newName)
        {
            Name = newName;
        }

        public Person()
        {
        }

        public Person( int id, string name )
        {
            Id = id;
            Name = name;
        }
    }

    public class PersonKeyAccessor : IKeyAccessor<Person>
    {
        public string GetId( Person actor )
        {
            return actor.Id.ToString();
        }

        public void SetId<TKey>( Person actor, TKey key )
        {
            
        }
    }

    public class ChangeName
    {
        
    }
}