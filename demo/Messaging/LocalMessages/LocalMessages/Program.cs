using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Messaging.Impl.Actors;
using Symbiote.StructureMap;
using Symbiote.Daemon;
using Symbiote.Log4Net;
using Symbiote.Messaging;

namespace LocalMessages
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Daemon( x => x.Name( "localChannelTest" ).Arguments( args ) )
                .AddConsoleLogger<Service>( x => x.Info().MessageLayout( m => m.Message().Newline() ) )
                .Messaging()
                .RunDaemon();
        }
    }

    public class Service : IDaemon
    {
        public IBus Bus { get; set; }

        public void Start()
        {
            Bus.Publish( new Message("Billy", "Jimmy","Hi") );
        }

        public void Stop()
        {

        }

        public Service( IBus bus )
        {
            Bus = bus;
            Bus.AddLocalChannel(x => x.CorrelateBy<Message>( m => m.To ));
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int MessageCount { get; set; }
        public readonly int Limit = 100000;
        public ChatService Chat { get; set; }

        public void TakeMessage(string from, string message)
        {
            "{0} told me, '{1}'"
                .ToInfo<Service>( from, message );

            if(++MessageCount < Limit)
            {
                Chat.Send( from, Name, "Hi!" );
            }
        }

        public Person( ChatService chat, string name )
        {
            Chat = chat;
            Name = name;
        }
    }

    public class PersonMessageHandler : IHandle<Person, Message>
    {
        public void Handle( Person actor, IEnvelope<Message> envelope )
        {
            actor.TakeMessage( envelope.Message.From, envelope.Message.Text );
        }
    }

    public class PersonKeyAccessor : IKeyAccessor<Person>
    {
        public string GetId( Person actor )
        {
            return actor.Name;
        }

        public void SetId<TKey>( Person actor, TKey id )
        {
            actor.Name = id.ToString();
        }
    }

    public class PersonFactory : IActorFactory<Person>
    {
        public ChatService Chat { get; set; }

        public Person CreateInstance<TKey>( TKey id )
        {
            return new Person( Chat, id.ToString() );
        }

        public PersonFactory( ChatService chat )
        {
            Chat = chat;
        }
    }

    public class ChatService
    {
        public IBus Bus { get; set; }

        public void Send(string to, string from, string message)
        {
            Bus.Publish( new Message(to, from, message) );
        }

        public ChatService( IBus bus )
        {
            Bus = bus;
        }
    }

    public class Message
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Text { get; set; }

        public Message() {}

        public Message( string to, string from, string text )
        {
            To = to;
            From = from;
            Text = text;
        }
    }
}
