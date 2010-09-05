using System.Diagnostics;
using Machine.Specifications;

namespace JsonRpc.Tests
{
    [Subject("Simple Action Call")]
    public class when_calling_simple_action : with_client
    {
        protected static Stopwatch stopwatch;

        private Because of = () =>
                                 {
                                     stopwatch = new Stopwatch();
                                     
                                     serviceMock
                                         .Setup(x => x.OneArgCall("hi"))
                                         .AtMostOnce();

                                     stopwatch.Start();
                                     proxy.Call(x => x.OneArgCall("hi"));
                                     stopwatch.Stop();
                                 };

        private It should_call_service_method = () => serviceMock.VerifyAll();
        private It should_take_less_than_half_a_second = () => stopwatch.ElapsedMilliseconds.ShouldBeLessThan(4);
    }
}