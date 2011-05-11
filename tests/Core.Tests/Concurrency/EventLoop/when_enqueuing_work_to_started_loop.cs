using System.Diagnostics;
using System.Linq;
using System.Threading;
using Machine.Specifications;
using Symbiote.Core.Extensions;

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

    public class when_enqueuing_one_million_ops_to_event_loop
        : with_event_loop
    {
        static int total;
        static Stopwatch watch;
        private Because of = () => { 
                                        Loop.Start( 1 );
                                        watch = Stopwatch.StartNew();
                                        Enumerable.Range( 0, 1000000 )
                                            .ForEach( x =>
                                                          {
                                                              Loop.Enqueue( () => total += 1 );
                                                          } );
                                        watch.Stop();
        };

        private It should_process_item = () => total.ShouldEqual( 1000000 );
        private It should_not_take_a_second = () => watch.ElapsedMilliseconds.ShouldBeLessThanOrEqualTo( 200 );
        private It should_have_dequeued_action = () => Loop.ActionQueue.Count.ShouldEqual( 0 );
    }
}