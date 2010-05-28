using System;
using System.Diagnostics;
using Machine.Specifications;

namespace Core.Tests.Reflection.POC
{
    public class when_reading_dynamically : with_precompiled_read_cache
    {
        protected static TestClass target = new TestClass();
        protected static string val1;
        protected static int val2;
        protected static TimeSpan val3;
        protected static Guid val4;
        protected static Stopwatch assignmentWatch;

        private Because of = () =>
                                 {
                                     assignmentWatch = Stopwatch.StartNew();

                                     val1 = readCache[Tuple.Create(typeof (TestClass), "val1")].Item2(target) as string;
                                     val2 = (int) readCache[Tuple.Create(typeof (TestClass), "val2")].Item2(target);
                                     val3 = (TimeSpan) readCache[Tuple.Create(typeof (TestClass), "val3")].Item2(target);
                                     val4 = (Guid) readCache[Tuple.Create(typeof (TestClass), "val4")].Item2(target);

                                     assignmentWatch.Stop();
                                     var ms = assignmentWatch.ElapsedMilliseconds;
                                 };

        private It should_match_val1 = () => val1.ShouldEqual(target.val1);
        private It should_match_val2 = () => val2.ShouldEqual(2);
        private It should_match_val3 = () => val3.ShouldEqual(TimeSpan.FromSeconds(3));
        private It should_match_val4 = () => val4.ShouldEqual(target.val4);
    }
}