using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Machine.Specifications;
using Symbiote.Core.Impl.Utility;
using Symbiote.Core.Extensions;

namespace Core.Tests.Utility.RingBuffer
{
    public class with_ring_buffer
        : with_assimilation
    {
        public static VolatileRingBuffer RingBuffer { get; set; }
        public static int Numbers { get; set; }
        public static Func<object, object> Callback { get; set; }

        private Establish context = () =>
        {
            RingBuffer = new VolatileRingBuffer( 100 );
            RingBuffer.AddTransform(x =>
            {
                Numbers++;
                return x;
            });
        };
    }

    public class when_cycling_ring_buffer_index
        : with_assimilation
    {
        public static RingBufferIndex Index { get; set; }
        public static Stopwatch Watch { get; set; }

        private Because of = () =>
        {
            Index = new RingBufferIndex(1, 10000);
            Watch = Stopwatch.StartNew();
            Enumerable.Range( 0, 999999 ).ForEach( x => Index++ );
            Watch.Stop();
        };

        private It should_equal_10 = () => Index.Value.ShouldEqual( 10 );
        private It should_take_less_than_40_ms = () => Watch.ElapsedMilliseconds.ShouldBeLessThan( 40 );
    }

    public class when_testing_ring_buffer_throughput
        : with_ring_buffer
    {
        public static Stopwatch BufferWatch { get; set; }

        private Because of = () =>
        {
            RingBuffer.Start();
            BufferWatch = Stopwatch.StartNew();
            Enumerable.Range( 0, 1000 ).ForEach( RingBuffer.Write );
            BufferWatch.Stop();
            RingBuffer.Stop();
        };
        
        private It should_have_1000_entries = () => Numbers.ShouldEqual( 100000 );
        private It should_take_1_ms = () => BufferWatch.ElapsedMilliseconds.ShouldEqual( 1 );
    }

    // 50 - 64 ms : 900 -> 936
}
