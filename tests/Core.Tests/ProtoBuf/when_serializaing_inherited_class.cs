using Machine.Specifications;
using Symbiote.Core.Serialization;

namespace Core.Tests.ProtoBuf
{
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
