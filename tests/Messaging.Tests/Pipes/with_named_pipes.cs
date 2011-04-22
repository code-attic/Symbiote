using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Messaging;

namespace Messaging.Tests.Pipes
{
    public class with_named_pipes
        : with_assimilation
    {
        public static IBus bus { get; set; }
        private Establish context = () =>
                                        {
                                            bus = Assimilate.GetInstanceOf<IBus>();
                                            bus.AddNamedPipeChannel( x => x.Name( "server" ).Pipe( "test").AsServer() );
                                            bus.AddNamedPipeChannel( x => x.Name( "client" ).Pipe( "test") );
                                        };
    }

    [Serializable]
    [DataContract]
    public class TestMessage
    {
        [DataMember(Order = 1)]
        public string Text { get; set; }
    }

    public class TestMessageHandler : IHandle<TestMessage>
    {
        public Action<IEnvelope> Handle( TestMessage message )
        {
            return x => x.Reply( "I got your message." );
        }
    }

    public class when_sending_messages_via_pipes
        : with_named_pipes
    {
        public static string result = null;

        private Because of = () =>
                                 {
                                     result = bus.Request<TestMessage, string>("client", new TestMessage() { Text = "Hi." }).WaitFor( 400 );
                                 };
        
        private It should_get_response = () => result.ShouldEqual( "I got your message." );
    }
}
