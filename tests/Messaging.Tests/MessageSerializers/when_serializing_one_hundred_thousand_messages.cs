using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Messaging.Impl.Serialization;
using Symbiote.StructureMap;

namespace Messaging.Tests.MessageSerializers
{
    public class when_serializing_one_hundred_thousand_messages
    {
        protected static Stopwatch json_serializer_watch { get; set; }
        protected static Stopwatch json_deserializer_watch { get; set; }
        protected static Stopwatch netbinary_serializer_watch { get; set; }
        protected static Stopwatch netbinary_deserializer_watch { get; set; }
        protected static Stopwatch protobuf_serializer_watch { get; set; }
        protected static Stopwatch protobuf_deserializer_watch { get; set; }
        protected static List<Message> deserializedJson { get; set; }
        protected static List<Message> deserializedBinary { get; set; }
        protected static List<Message> deserializedProtobuf { get; set; }

        private Because of = () =>
                                 {
                                     Assimilate.Core<StructureMapAdapter>();

                                     var messages =
                                         Enumerable.Range(0, 10)
                                         .Select(x => new Message() {Id = Guid.NewGuid(), Text = "This is a message with stuff!"})
                                         .ToList();

                                     //Prime
                                     var message = new Message() {Id = Guid.Empty, Text = "Empty"};
                                     message.ToJson().FromJson<Message>();

                                     NetBinarySerializer binarySerializer = new NetBinarySerializer();
                                     binarySerializer.Deserialize<Message>(binarySerializer.Serialize(message));

                                     ProtobufMessageSerializer protobufSerializer = new ProtobufMessageSerializer();
                                     protobufSerializer.Deserialize<Message>(protobufSerializer.Serialize(message));

                                     //json
                                     json_serializer_watch = Stopwatch.StartNew();
                                     var jsonStreams = messages.Select(x => x.ToJson()).ToList();
                                     json_serializer_watch.Stop();

                                     json_deserializer_watch = Stopwatch.StartNew();
                                     deserializedJson = jsonStreams.Select(x => x.FromJson<Message>()).ToList();
                                     json_deserializer_watch.Stop();

                                     //netbinary
                                     netbinary_serializer_watch = Stopwatch.StartNew();
                                     var byteArrays = messages.Select(binarySerializer.Serialize).ToList();
                                     netbinary_serializer_watch.Stop();

                                     netbinary_deserializer_watch = Stopwatch.StartNew();
                                     deserializedBinary = byteArrays.Select(binarySerializer.Deserialize<Message>).ToList();
                                     netbinary_deserializer_watch.Stop();

                                     //protobuf
                                     protobuf_serializer_watch = Stopwatch.StartNew();
                                     var protobufs = messages.Select(protobufSerializer.Serialize).ToList();
                                     protobuf_serializer_watch.Stop();

                                     protobuf_deserializer_watch = Stopwatch.StartNew();
                                     deserializedProtobuf = protobufs.Select(protobufSerializer.Deserialize<Message>).ToList();
                                     protobuf_deserializer_watch.Stop();
                                 };

        private It should_deserialize_all_via_binary = () => deserializedBinary.Count.ShouldEqual(100000);
        private It should_deserialize_all_via_json = () => deserializedJson.Count.ShouldEqual(100000);
        private It should_deserialize_all_via_protofbuf = () => deserializedProtobuf.Count.ShouldEqual(100000);

        private It protobuf_should_deserialize_too_fast =
            () => protobuf_deserializer_watch.ElapsedMilliseconds.ShouldBeLessThan(100);

        private It protobuf_should_serialize_too_fast =
            () => protobuf_serializer_watch.ElapsedMilliseconds.ShouldBeLessThan(100);

        private It binary_should_deserialize_too_fast =
            () => netbinary_deserializer_watch.ElapsedMilliseconds.ShouldBeLessThan(100);

        private It binary_should_serialize_too_fast =
            () => netbinary_serializer_watch.ElapsedMilliseconds.ShouldBeLessThan(100);

        private It json_should_deserialize_too_fast =
            () => json_deserializer_watch.ElapsedMilliseconds.ShouldBeLessThan(100);

        private It json_should_serialize_too_fast =
            () => json_serializer_watch.ElapsedMilliseconds.ShouldBeLessThan(100);
    }

    [DataContract]
    [Serializable]
    public class Message
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember(Order = 2)]
        public string Text { get; set; }
    }
}
