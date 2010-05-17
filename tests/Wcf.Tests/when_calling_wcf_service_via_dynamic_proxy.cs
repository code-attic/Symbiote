using System;
using Machine.Specifications;

namespace Wcf.Tests
{
    public class when_calling_wcf_service_via_dynamic_proxy : with_wcf_client
    {
        protected static bool result;

        private Because of = () =>
                                 {
                                     var dateTime = DateTime.Now;
                                     var newGuid = Guid.NewGuid();
                                     result = service.Call(x => x.TwoArgCall(dateTime, newGuid));
                                     watch.Stop();
                                     host.Stop();
                                 };
        
        private It should_run_in_time = () => watch.ElapsedMilliseconds.ShouldBeLessThan(100);
        private It should_return_true = () => result.ShouldBeTrue();
    }
}