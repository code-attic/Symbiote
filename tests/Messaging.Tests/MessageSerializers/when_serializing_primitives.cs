using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Messaging.Impl.Serialization;

namespace Messaging.Tests.MessageSerializers
{
    public class when_serializing_primitives
    {
        protected static decimal original = 10.5m;
        protected static decimal result;

        private Because of = () =>
        {
            var serializer = new ProtobufMessageSerializer();
            var temp = serializer.Serialize( original );
            result = serializer.Deserialize<decimal>( temp );
        };
        
        private It should_get_value_back = () => original.ShouldEqual( result );
    }
}
