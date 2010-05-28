using System;
using System.Diagnostics;
using Machine.Specifications;
using Symbiote.Core.Reflection;

namespace Core.Tests.Reflection
{
    public class when_writing_dynamically 
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

                                     Reflector.WriteMember(target, "val1", val1);
                                     Reflector.WriteMember(target, "val2", val2);
                                     Reflector.WriteMember(target, "val3", val3);
                                     Reflector.WriteMember(target, "val4", val4);

                                     assignmentWatch.Stop();
                                     var ms = assignmentWatch.ElapsedMilliseconds;
                                 };

        private It should_match_val1 = () => target.val1.ShouldEqual(val1);
        private It should_match_val2 = () => target.GetVal2().ShouldEqual(val2);
        private It should_match_val3 = () => target.GetVal3().ShouldEqual(val3);
        private It should_match_val4 = () => target.val4.ShouldEqual(val4);
    }
}