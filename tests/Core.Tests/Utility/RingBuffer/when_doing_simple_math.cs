using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Machine.Specifications;

namespace Core.Tests.Utility.RingBuffer
{
    public class when_doing_simple_math
        : with_buffer
    {
        private static int TOTAL_WRITES = 1000000;
        public static List<int> Counts;
        public static Stopwatch Watch { get; set; }
        private Because of = () =>
        {
            Counts = new List<int>(TOTAL_WRITES);
            Buffer.AddTransform(x => (int)x + 5);
            Buffer.AddTransform(x => (int)x * 10);
            Buffer.AddTransform(x =>
            {
                Counts.Add((int)x);
                return x;
            });
            Watch = Stopwatch.StartNew();
            Buffer.Start();
            Enumerable
                .Range(0, TOTAL_WRITES)
                .AsParallel()
                .ForAll(x => Buffer.Write(5));
            
            Thread.Sleep( 10 );
            Buffer.Stop();
            Watch.Stop();
        };

        private It should_have_completed_all_steps = () => ShouldExtensionMethods.ShouldBeTrue( Counts.All(x => x == 100) );
        private It should_have_all_items = () => Counts.Count.ShouldEqual(TOTAL_WRITES);
        private It should_take_1_second = () => Watch.ElapsedMilliseconds.ShouldBeLessThan(1500);
    }
}