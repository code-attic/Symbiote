using System;
using Machine.Specifications;
using Symbiote.Core.Impl.Serialization;
using Symbiote.Messaging.Impl.Serialization;

namespace Messaging.Tests.MessageSerializers
{
    public class when_detecting_serialization_options_for_json_only_message
    {
        private static Type bestSerializer;

        private Because of = () =>
        {
            bestSerializer = MessageSerializer.GetBestMessageSerializerFor<JsonOnlyMessage>();
        };

        private It should_have_default_constructor = () => ShouldExtensionMethods.ShouldBeTrue( typeof(JsonOnlyMessage).HasDefaultConstructor() );
        private It should_be_binary_serializable = () => ShouldExtensionMethods.ShouldBeFalse( typeof(JsonOnlyMessage).IsBinarySerializable() );
        private It should_be_protobuf_serializable = () => ShouldExtensionMethods.ShouldBeFalse( typeof(JsonOnlyMessage).IsProtobufSerializable() );
        private It should_be_json_serializable = () => ShouldExtensionMethods.ShouldBeTrue( typeof(JsonOnlyMessage).IsJsonSerializable() );
        private It should_recommend_json_serializer = () => bestSerializer.ShouldEqual(typeof(JsonMessageSerializer));
    }
}