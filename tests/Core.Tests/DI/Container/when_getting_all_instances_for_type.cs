using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Core.Tests.DI.Container
{
    public class when_getting_all_instances_for_type : with_multiple_classes_register_to_marker
    {
        public static List<IMark> instances;
        private static Type[] types = new[] { typeof(Multiple1), typeof(Multiple2), typeof(Multiple3) };
        private Because of = () =>
            {
                instances = Container.GetAllInstances<IMark>().ToList();
            };

        private It should_have_3_instances = () => instances.Count.ShouldEqual( 3 );
        private It should_have_an_instance_of_each_type = () => instances.Select( x => x.GetType() ).ShouldContain( types );
    }
}
