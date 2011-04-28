using Machine.Specifications;

namespace Core.Tests.Concurrency.EventLoop
{
    public class when_enqueuing_work_to_stopped_loop
        : with_event_loop
    {
        static int total;

        private Because of = () => { 
                                       Loop.Enqueue( () => total += 1 );
        };

        private It should_not_process_item = () => total.ShouldEqual( 0 );
        private It should_have_action_enqueued = () => Loop.ActionQueue.Count.ShouldEqual( 1 );
    }
}