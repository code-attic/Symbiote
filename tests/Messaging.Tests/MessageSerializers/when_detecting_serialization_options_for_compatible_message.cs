using System;
using Machine.Specifications;
using Symbiote.Core.Impl.Serialization;
using Symbiote.Messaging.Impl.Serialization;

namespace Messaging.Tests.MessageSerializers
{
    public class when_detecting_serialization_options_for_compatible_message
    {
        private static Type bestSerializer;

        private Because of = () =>
        {
            bestSerializer = MessageSerializer.GetBestMessageSerializerFor<CompatibleMessage>();
        };

        private It should_have_default_constructor = () => ShouldExtensionMethods.ShouldBeTrue( typeof(CompatibleMessage).HasDefaultConstructor() );
        private It should_be_binary_serializable = () => ShouldExtensionMethods.ShouldBeTrue( typeof(CompatibleMessage).IsBinarySerializable() );
        private It should_be_protobuf_serializable = () => ShouldExtensionMethods.ShouldBeTrue( typeof(CompatibleMessage).IsProtobufSerializable() );
        private It should_be_json_serializable = () => ShouldExtensionMethods.ShouldBeTrue( typeof(CompatibleMessage).IsJsonSerializable() );
        private It should_recommend_protobuf_serializer = () => bestSerializer.ShouldEqual( typeof(ProtobufMessageSerializer) );
    }
}