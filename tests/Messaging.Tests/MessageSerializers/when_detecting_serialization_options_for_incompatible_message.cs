using System;
using Machine.Specifications;
using Symbiote.Core.Serialization;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Serialization;

namespace Messaging.Tests.MessageSerializers
{
    public class when_detecting_serialization_options_for_incompatible_message
    {
        private static Type bestSerializer;
        private static Exception exception;

        private Because of = () =>
        {
            exception = Catch.Exception(() => 
                                        bestSerializer = MessageSerializer.GetBestMessageSerializerFor<IncompatibleMessage>());
        };

        private It should_have_default_constructor = () => ShouldExtensionMethods.ShouldBeFalse( typeof(IncompatibleMessage).HasDefaultConstructor() );
        private It should_be_binary_serializable = () => ShouldExtensionMethods.ShouldBeFalse( typeof(IncompatibleMessage).IsBinarySerializable() );
        private It should_be_protobuf_serializable = () => ShouldExtensionMethods.ShouldBeFalse( typeof(IncompatibleMessage).IsProtobufSerializable() );
        private It should_be_json_serializable = () => ShouldExtensionMethods.ShouldBeFalse( typeof(IncompatibleMessage).IsJsonSerializable() );
        private It should_throw_exception = () => exception.ShouldBeOfType<MessagingException>();
    }
}