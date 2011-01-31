using System.Diagnostics;
using System.IO;
using System.Net;
using Machine.Specifications;
using Symbiote.Core.Extensions;
using Symbiote.Core.Serialization;

namespace JsonRpc.Tests
{
    class when_calling_action_natively : with_server
    {
        protected static Stopwatch stopwatch;

        private Because of = () =>
        {
            stopwatch = new Stopwatch();

            serviceMock
                .Setup(x => x.OneArgCall("hi"))
                .AtMostOnce();


            stopwatch.Start();

            var request = WebRequest.Create(@"http://localhost:8420/ITestService/OneArgCall");
            request.Method = "POST";
            var args = new {arguments = new {arg1 = "hi"}};
            using(var stream = request.GetRequestStream())
            using(var writer = new StreamWriter(stream))
            {
                writer.Write(args.ToJson(false));
                writer.Flush();
            }
            var response = request.GetResponse();

            stopwatch.Stop();
        };

        private It should_call_service_method = () => serviceMock.VerifyAll();
        private It should_take_less_than_half_a_second = () => stopwatch.ElapsedMilliseconds.ShouldBeLessThan(350);
    }
}
