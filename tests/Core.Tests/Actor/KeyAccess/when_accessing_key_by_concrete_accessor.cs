using Machine.Specifications;

namespace Core.Tests.Actor.KeyAccess
{
    public class when_accessing_key_by_concrete_accessor
        : with_key_accessor
    {
        public static ConcreteType instance { get; set; }
        public static string expected = "test";
        public static string Id { get; set; }
        private Because of = () =>
                                 {
                                     instance = new ConcreteType();
                                     KeyAccessor.SetId( instance, expected );
                                     Id = KeyAccessor.GetId( instance );
                                 };

        private It should_have_correct_id = () => Id.ShouldEqual( expected );
    }
}