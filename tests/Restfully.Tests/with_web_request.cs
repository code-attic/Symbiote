using System.Net;
using Machine.Specifications;

namespace JsonRpc.Tests
{
    public abstract class with_web_request : with_http_listener
    {
        protected static string requestUri;
        protected static WebRequest request;

        private Establish context = () =>
                                        {
                                            requestUri = baseUri + @"controller/action/arg1/arg2?a=1&b=2";
                                            request = WebRequest.Create(requestUri);
                                        };
    }
}