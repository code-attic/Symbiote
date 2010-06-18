using System.IO;
using System.Net;
using Machine.Specifications;

namespace Restfully.Tests
{
    [Subject("simple request")]
    public class when_simple_get_request : with_web_request
    {
        protected static WebResponse response;
        protected static string result;

        private Because of = () =>
                                 {
                                     request.Method = "GET";
                                     request.ContentType = "text/plain";
                                     //using (var stream = request.GetRequestStream())
                                     //using (var writer = new StringWriter())
                                     //{
                                     //    writer.WriteLine("Anyone home?");
                                     //    writer.Flush();
                                     //}
                                     response = request.GetResponse();
                                     using (var reader = new StreamReader(response.GetResponseStream()))
                                     {
                                         result = reader.ReadToEnd();
                                     }
                                 };

        private It should_have_uri_in_context = () => httpContext.Request.Url.AbsoluteUri.ShouldEqual(requestUri);
        private It should_have_path_in_context = () => relativePath.ShouldEqual(@"/controller/action/arg1/arg2");
        private It should_have_query_string_in_context = () => queryString.ShouldEqual(@"?a=1&b=2");
        private It should_have_response = () => result.ShouldEqual("howdy");


    }
}