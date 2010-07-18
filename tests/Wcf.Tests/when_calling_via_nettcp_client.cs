using System;
using Machine.Specifications;

namespace Wcf.Tests
{
    public class when_calling_via_nettcp_client : with_wcf_nettcp_client
    {
        protected static Return result;
        protected static DateTime dateTime;
        protected static Guid newGuid;

        private Because of = () =>
                                 {
                                     dateTime = DateTime.Now;
                                     newGuid = Guid.NewGuid();
                                     result = service.Call(x => x.TwoArgCall(dateTime, newGuid));
                                     result = service.Call(x => x.TwoArgCall(dateTime, newGuid));
                                     watch.Stop();
                                     host.Stop();
                                 };

        private It should_run_in_time = () => watch.ElapsedMilliseconds.ShouldBeLessThan(800);
        private It should_return_correct_results = () => result.datetime.ShouldEqual(dateTime);
    }
}