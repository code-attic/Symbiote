using System;
using System.Diagnostics;
using Core.Tests.Reflection.POC;
using Machine.Specifications;
using Symbiote.Core.Reflection;

namespace Core.Tests.Reflection
{
    public class when_reading_dynamically 
    {
        protected static TestClass target = new TestClass();
        protected static string val1;
        protected static int val2;
        protected static TimeSpan val3;
        protected static Guid val4;
        protected static string val5a;
        protected static decimal val5b;
        protected static Stopwatch assignmentWatch;

        private Because of = () =>
                                 {
                                     assignmentWatch = Stopwatch.StartNew();

                                     val1 = Reflector.ReadMember<string>(target, "val1");
                                     val2 = Reflector.ReadMember<int>(target, "val2");
                                     val3 = Reflector.ReadMember<TimeSpan>(target, "val3");
                                     val4 = Reflector.ReadMember<Guid>(target, "val4");
                                     val5a = Reflector.ReadMember<string>( target, "val5.val5a" );
                                     val5b = Reflector.ReadMember<decimal>( target, "val5.val5b" );

                                     assignmentWatch.Stop();
                                     var ms = assignmentWatch.ElapsedMilliseconds;
                                 };

        private It should_match_val1 = () => val1.ShouldEqual(target.val1);
        private It should_match_val2 = () => val2.ShouldEqual(2);
        private It should_match_val3 = () => val3.ShouldEqual(TimeSpan.FromSeconds(3));
        private It should_match_val4 = () => val4.ShouldEqual(target.val4);
        private It should_match_val5a = () => val5a.ShouldEqual( "child" );
        private It should_match_val5b = () => val5b.ShouldEqual( 10.1m );
    }
}