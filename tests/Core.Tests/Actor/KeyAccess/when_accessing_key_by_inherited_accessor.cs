using Machine.Specifications;

namespace Core.Tests.Actor.KeyAccess
{
    public class when_accessing_key_by_inherited_accessor
        : with_key_accessor
    {
        public static Class1 instance { get; set; }
        public static string expected = "test";
        public static string Id { get; set; }
        private Because of = () =>
                                 {
                                     instance = new Class1();
                                     KeyAccessor.SetId(instance, expected);
                                     Id = KeyAccessor.GetId(instance);
                                 };

        private It should_have_correct_id = () => Id.ShouldEqual(expected);
    }
}