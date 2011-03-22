using System.Text;
using Machine.Specifications;

namespace Http.Tests
{
    public abstract class with_full_uri_get_request
        : with_timer
    {
        public static string request_body;
        public static byte[] bytes;

        private Establish context = () =>
                                        {
                                            request_body =
                                                @"GET http://localhost:8080/fun/times?id=101&rev=234 HTTP/1.1
Content-Type: text/plain
";
                                            bytes = Encoding.UTF8.GetBytes( request_body );
                                        };
    }
}