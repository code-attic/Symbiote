using System;
using Machine.Specifications;
using Symbiote.Core.Serialization;
using Symbiote.Messaging.Impl.Serialization;

namespace Messaging.Tests.MessageSerializers
{
    public class when_detecting_serialization_options_for_json_and_binary_message
    {
        private static Type bestSerializer;

        private Because of = () =>
        {
            bestSerializer = MessageSerializer.GetBestMessageSerializerFor<BinaryAndJsonMessage>();
        };

        private It should_have_default_constructor = () => ShouldExtensionMethods.ShouldBeTrue( typeof(BinaryAndJsonMessage).HasDefaultConstructor() );
        private It should_be_binary_serializable = () => ShouldExtensionMethods.ShouldBeTrue( typeof(BinaryAndJsonMessage).IsBinarySerializable() );
        private It should_be_protobuf_serializable = () => ShouldExtensionMethods.ShouldBeFalse( typeof(BinaryAndJsonMessage).IsProtobufSerializable() );
        private It should_be_json_serializable = () => ShouldExtensionMethods.ShouldBeTrue( typeof(BinaryAndJsonMessage).IsJsonSerializable() );
        private It should_recommend_binary_serializer = () => bestSerializer.ShouldEqual( typeof(NetBinarySerializer) );
    }
}