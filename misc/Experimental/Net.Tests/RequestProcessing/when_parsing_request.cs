using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Net;

namespace Net.Tests.RequestProcessing
{
    public abstract class with_put_request
    {
        protected static string requestBody;

        private Establish context = () =>
                                        {
                                            requestBody =
@"PUT /test/item?id=10 HTTP/1.0
Host: localhost

{
    value: 10,
    message: ""hi""
}
";
                                        };
    }

    public class when_parsing_put_request : with_put_request
    {
        protected static HttpRequest request;
        protected static Stopwatch watch;
        protected static string expectedBody = @"{
    value: 10,
    message: ""hi""
}
";
        private Because of = () =>
        {
            watch = Stopwatch.StartNew();
            request = new HttpRequest(requestBody);
            watch.Stop();
        };

        private It should_be_http_protocol = () => request.Protocol.ShouldEqual("HTTP");
        private It should_be_version_10 = () => request.Version.ShouldEqual(Version.Parse("1.0"));
        private It should_be_get_verb = () => request.Verb.ShouldEqual("PUT");
        private It should_have_correct_path = () => request.Path.ShouldEqual(@"/test/item");
        private It should_have_query_string = () => request.Query.ShouldEqual("id=10");
        private It should_be_localhost = () => request.Host.ShouldEqual("localhost");
        private It should_have_full_url = () => request.RawUrl.ToLower().ShouldEqual(@"http://localhost/test/item?id=10");
        private It should_contain_request_body = () => request.Body.ShouldEqual(expectedBody);
        private It should_take_only_25ms = () => watch.ElapsedMilliseconds.ShouldBeLessThan(25);
    }


    public abstract class with_simple_get_request
    {
        protected static string requestBody;

        private Establish context = () =>
        {
            requestBody =
@"GET /test/item?id=10 HTTP/1.1
Host: localhost

";
        };
    }

    public class when_parsing_simple_get_request : with_simple_get_request
    {
        protected static HttpRequest request;
        protected static Stopwatch watch;

        private Because of = () =>
                                 {
                                     watch = Stopwatch.StartNew();
                                     request = new HttpRequest(requestBody);
                                     watch.Stop();
                                 };
        
        private It should_be_http_protocol = () => request.Protocol.ShouldEqual("HTTP");
        private It should_be_version_11 = () => request.Version.ShouldEqual(Version.Parse("1.1"));
        private It should_be_get_verb = () => request.Verb.ShouldEqual("GET");
        private It should_have_correct_path = () => request.Path.ShouldEqual(@"/test/item");
        private It should_have_query_string = () => request.Query.ShouldEqual("id=10");
        private It should_be_localhost = () => request.Host.ShouldEqual("localhost");
        private It should_have_full_url = () => request.RawUrl.ToLower().ShouldEqual(@"http://localhost/test/item?id=10");
        private It should_take_only_25ms = () => watch.ElapsedMilliseconds.ShouldBeLessThan(25);
    }
}
