using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Machine.Specifications;
using RabbitMQ.Client.Framing.v0_9_1;
using Symbiote.Core.Serialization;
using Symbiote.Rabbit;

namespace Rabbit.Tests.Envelope
{
    [DataContract]
    public class TestMessage
    {
        [DataMember(Order = 1)]
        public string Content { get; set; }

        public TestMessage( string content )
        {
            Content = content;
        }
    }

    public class when_starting_with_untyped_envelope
        : with_assimilation
    {
        private Because of = () =>
        {
            var untyped = new RabbitEnvelope(
                "",
                new BasicProperties()
                {
                    ContentEncoding = "plain/text",
                    CorrelationId = "",
                    Headers = new Dictionary<string, string>
                    {
                        {"Position","1"},
                        {"Sequence","1"},
                        {"SequenceEnd","false"},
                        {"MessageType","TestMessage"},
                    }
                },
                0,
                "",
                null,
                false,
                "",
                new TestMessage("test").ToProtocolBuffer()
                );
        };
    }
}
