using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Core.Tests.DI.Container
{
    public interface IAmNotDefined
    {
    }

    public class when_asking_for_non_defined_type : with_container
    {
        static Exception exception;

        private Because of = () => 
        {
            exception = Catch.Exception( () =>
                                             {
                                                 var instance = Container.GetInstance<IAmNotDefined>();
                                             } );
        };

        private It should_be_argument_exception = () => exception.ShouldBe( typeof( ArgumentException ) );
    }

    public class when_asking_for_non_defined_type_list : with_container
    {
        static Exception exception;
        static List<IAmNotDefined> list;

        private Because of = () => 
        {
            exception = Catch.Exception( () =>
                                             {
                                                 list = Container.GetAllInstances<IAmNotDefined>().ToList();
                                             } );
        };

        private It should_be_null = () => exception.ShouldBeNull();
        private It should_return_empty_list = () => list.Count.ShouldEqual( 0 );
    }
}
