using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Machine.Specifications;
using Symbiote.Messaging.Impl.Serialization;
using Symbiote.Core.Serialization;

namespace Messaging.Tests.MessageSerializers
{

    [Serializable]
    [DataContract]
    public class Message
    {
        [DataMember(IsRequired=false,Order=1)]
        public string Text { get; set; }
        public Message() {}
    }
    public class when_detecting_serialization_options_for_message
    {
        protected static Message message;

        private Because of = () =>
        {
            
        };

        private It should_have_default_constructor = () => typeof(Message).HasDefaultConstructor().ShouldBeTrue();
        private It should_be_binary_serializable = () => typeof(Message).IsBinarySerializable().ShouldBeTrue();
        private It should_be_protobuf_serializable = () => typeof(Message).IsProtobufSerializable().ShouldBeTrue();
        private It should_be_json_serializable = () => typeof(Message).IsJsonSerializable().ShouldBeTrue();
    }
}
