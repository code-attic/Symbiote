using System.Runtime.Serialization;
using Machine.Specifications;
using Symbiote.Core.Impl.Serialization;
using ProtoBuf;

namespace Core.Tests.ProtoBuf
{
    [DataContract]
    [ProtoInclude(1, typeof(Class))]
    public class Base
    {
        [DataMember(Order = 10)]
        public string BaseProperty { get; set; }
    }

    [DataContract]
    public class Class : Base
    {
        [DataMember(Order = 20)]
        public string ClassProperty { get; set; }
    }

    public class when_serializaing_inherited_class
    {
        public static Class instance { get; set; }

        private Because of = () =>
        {
            var a = new Class()
            {
                BaseProperty = "base",
                ClassProperty = "instance"
            };

            var bytes = a.ToProtocolBuffer();

            instance = bytes.FromProtocolBuffer<Class>();
        };

        private It should_have_class_property = () => instance.ClassProperty.ShouldEqual( "instance" );
        private It should_have_base_property = () => instance.BaseProperty.ShouldEqual( "base" );
    }
}
