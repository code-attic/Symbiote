using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
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


        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }


    }

    public class Message
    {
        public string From { get; set; }
        public string Text { get; set; }

        public Message() {}

        public Message( string @from, string text )
        {
            From = from;
            Text = text;
        }
    }
}
