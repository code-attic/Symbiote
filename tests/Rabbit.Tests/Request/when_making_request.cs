using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Machine.Specifications;
using Symbiote.Messaging;
using Symbiote.Rabbit;

namespace Rabbit.Tests.Request
{
    public class when_making_request
        : with_rabbit_configuration
    {
        public static Reply Reply { get; set; }

        private Because of = () =>
        {
            Bus.AddRabbitChannel(x => x.Direct("request").AutoDelete());
            Bus.AddRabbitQueue(x => x.ExchangeName("request").QueueName("request").NoAck().AutoDelete().StartSubscription());
            Reply = Bus.Request<Request, Reply>("request", new Request()).WaitFor( 55 );
        };
        
        private It should_have_response = () => Reply.Text.ShouldEqual("I have an answer!");
    }

    [DataContract]
    public class Request
    {
        [DataMember(IsRequired = false, Order = 1)]
        public string Text { get; set; }
    }

    [DataContract]
    public class Reply
    {
        [DataMember(IsRequired = false, Order = 1)]
        public string Text { get; set; }
    }

    public class RequestHandler : IHandle<Request>
    {
        public void Handle(IEnvelope<Request> envelope)
        {
            envelope.Reply(new Reply()
            {
                Text = "I have an answer!"
            });
        }
    }
}
