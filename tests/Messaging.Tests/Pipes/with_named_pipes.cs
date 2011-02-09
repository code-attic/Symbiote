using System;
using System.Collections.Generic;
using System.Linq;
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

                                            bus.AddNamedPipeListener( x => x.Named( "test" ).StartSubscription() );
                                            bus.AddNamedPipeChannel( x => x.Name( "test" ) );
                                        };
    }

    public class TestMessage
    {
        public string Text { get; set; }
    }

    public class TestMessageHandler : IHandle<TestMessage>
    {
        public Action<IEnvelope> Handle( TestMessage message )
        {
            return x => x.Reply( "I got your mesage." );
        }
    }

    public class when_sending_messages_via_pipes
        : with_named_pipes
    {
        public static string result = null;

        private Because of = () =>
                                 {
                                     result = bus.Request<TestMessage, string>( "test", new TestMessage() {Text = "Hi."} ).WaitFor( 10000 );
                                 };
        
        private It should_get_response = () => result.ShouldEqual( "I got your message." );
    }
}
