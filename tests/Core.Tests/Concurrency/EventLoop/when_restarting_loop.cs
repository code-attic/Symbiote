using System.Threading;
using Machine.Specifications;

namespace Core.Tests.Concurrency.EventLoop
{
    public class when_restarting_loop
        : with_event_loop
    {
        static int total;

        private Because of = () => { 
                                       Loop.Enqueue( () => total += 1 );
                                       Loop.Start( 1 );
                                       Loop.Stop();
                                       Loop.Enqueue( () => total += 1 );
                                       Loop.Enqueue( () => total += 1 );
                                       Loop.Enqueue( () => total += 1 );
                                       Loop.Start( 1 );
                                       Thread.Sleep( 2 );
        };

        private It should_process_item = () => total.ShouldEqual( 4 );
        private It should_have_dequeued_action = () => Loop.ActionQueue.Count.ShouldEqual( 0 );
    }
}