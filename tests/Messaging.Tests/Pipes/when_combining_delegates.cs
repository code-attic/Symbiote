using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Messaging.Impl.Transform;
using Symbiote.StructureMap;

namespace Messaging.Tests.Pipes
{
    public class when_combining_delegates
    {
        protected static int test1;
        protected static int test2;
        protected static int test3;

        private Establish context = () =>
        {
            Assimilate.Core<StructureMapAdapter>();
        };

        private Because of = () =>
        {
            test1 = Pipeline
                .New<IntToString, int, string>()
                .Then(new StringToBytes())
                .Then<BytesToString, string>()
                .Then<StringToInt, int>()
                .Process( 10 );

            test2 = new StringToInt()
                .WireUp( new BytesToString() )
                .WireUp( new StringToBytes() )
                .WireUp( new IntToString() )
                .Process( 10 );

            var transformer = new Transformer()
                .Then<TransformIntToString>()
                .Then<TransformStringToBytes>();

            var temp = transformer.Transform<int, byte[]>( 10 );
            test3 = transformer.Reverse<byte[], int>( temp );
        };

        private It should_get_10_for_test1 = () => test1.ShouldEqual( 10 );
        private It should_get_10_for_test2 = () => test2.ShouldEqual( 10 );
        private It should_get_10_for_test3 = () => test3.ShouldEqual( 10 );
    }
}