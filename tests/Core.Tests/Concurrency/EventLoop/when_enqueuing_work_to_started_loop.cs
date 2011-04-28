using Machine.Specifications;

namespace Core.Tests.Concurrency.EventLoop
{
    public class when_enqueuing_work_to_started_loop
        : with_event_loop
    {
        static int total;

        private Because of = () => { 
                                       Loop.Start( 1 );
                                       Loop.Enqueue( () => total += 1 );
                                       //Thread.Sleep( 2 );
        };

        private It should_process_item = () => total.ShouldEqual( 1 );
        private It should_have_dequeued_action = () => Loop.ActionQueue.Count.ShouldEqual( 0 );
    }
}