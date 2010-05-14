using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Machine.Specifications;

namespace Restfully.Tests
{
    [Subject("Simple Function Call")]
    public class when_calling_simple_func : with_client
    {
        protected static Stopwatch stopwatch;
        protected static DateTime time;
        protected static Guid id;
        protected static bool result;

        private Because of = () =>
                                 {
                                     stopwatch = new Stopwatch();

                                     id = Guid.NewGuid();
                                     time = DateTime.Now;

                                     serviceMock
                                         .Setup(x => x.TwoArgCall(time, id))
                                         .Returns(true)
                                         .AtMost(1);

                                     stopwatch.Start();
                                     result = proxy.Call(x => x.TwoArgCall(time,id));
                                     stopwatch.Stop();
                                 };

        private It should_call_service_method = () => serviceMock.VerifyAll();
        private It should_take_less_than_half_a_second = () => stopwatch.ElapsedMilliseconds.ShouldBeLessThan(400);
        private It should_return_true = () => result.ShouldBeTrue();
    }
}