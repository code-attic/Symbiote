using System.Text;
using Machine.Specifications;

namespace Http.Tests
{
    public abstract class with_relative_put_request
        : with_timer
    {
        public static string request_body;
        public static byte[] bytes;

        private Establish context = () =>
                                        {
                                            request_body =
                                                @"PUT /fun/times/100 HTTP/1.1
Host: localhost:8080
Content-Type: text/plain
Content-Length: 11

This is fun";

                                            bytes = Encoding.UTF8.GetBytes( request_body );
                                        };
    }
}