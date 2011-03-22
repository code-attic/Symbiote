using System.Text;
using Machine.Specifications;

namespace Http.Tests
{
    public abstract class with_relative_get_request
        : with_timer
    {
        public static string request_body;
        public static byte[] bytes;

        private Establish context = () =>
                                        {
                                            request_body =
                                                @"GET /fun/times?id=101&rev=234 HTTP/1.1
Host: localhost:8080

";

                                            bytes = Encoding.UTF8.GetBytes( request_body );
                                        };
    }
}