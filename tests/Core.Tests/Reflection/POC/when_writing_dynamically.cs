using System;
using System.Diagnostics;
using Machine.Specifications;

namespace Core.Tests.Reflection.POC
{
    public class when_writing_dynamically : with_precompiled_read_cache
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

                                     val1 = "4";
                                     val2 = 3;
                                     val3 = TimeSpan.FromSeconds(2);
                                     val4 = new Guid("00000000-0000-0000-0000-000000000001");

                                     writeCache[Tuple.Create(typeof(TestClass), "val1")].Item2(target, val1);
                                     writeCache[Tuple.Create(typeof(TestClass), "val2")].Item2(target, val2);
                                     writeCache[Tuple.Create(typeof(TestClass), "val3")].Item2(target, val3);
                                     writeCache[Tuple.Create(typeof(TestClass), "val4")].Item2(target, val4);

                                     assignmentWatch.Stop();
                                     var ms = assignmentWatch.ElapsedMilliseconds;
                                 };

        private It should_match_val1 = () => target.val1.ShouldEqual(val1);
        private It should_match_val2 = () => target.GetVal2().ShouldEqual(val2);
        private It should_match_val3 = () => target.GetVal3().ShouldEqual(val3);
        private It should_match_val4 = () => target.val4.ShouldEqual(val4);
    }
}