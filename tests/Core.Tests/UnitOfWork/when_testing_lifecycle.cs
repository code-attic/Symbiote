using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.UnitOfWork;

namespace Core.Tests.UnitOfWork
{
    public class when_testing_lifecycle
        : with_assimilation
    {
        static int count;
        private Because of = () => 
                                 { 
                                     var instance = Assimilate.GetInstanceOf<IEventListenerManager>();
                                     instance = Assimilate.GetInstanceOf<IEventListenerManager>();

                                     count = EventListenerManager.Instance;
                                 };

        private It should_result_in_singleton_instances = () => count.ShouldEqual( 1 );
    }
}