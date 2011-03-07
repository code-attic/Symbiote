using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Http.Impl.Adapter;
using Symbiote.Http.Impl.Adapter.SocketListener;
using Symbiote.Http.Owin;

namespace Http.Tests
{
    public abstract class with_timer
    {
        public static Stopwatch Timer { get; set; }
    }

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

    public class when_parsing_relative_get_request
        : with_relative_get_request
    {
        public static Request request;
        
        private Because of = () =>
            {
                Timer = Stopwatch.StartNew();
                request = RequestParser.CreateRequest( new ArraySegment<byte>( bytes ) );
                Timer.Stop();
            };
        
        private It should_have_correct_method = () => request.Method.ShouldEqual( "GET" );
        private It should_have_correct_base_url = () => request.BaseUri.ShouldEqual( @"HTTP://localhost:8080" );
        private It should_have_first_segment = () => request.PathSegments[0].ShouldEqual( @"fun" );
        private It should_have_second_segment = () => request.PathSegments[1].ShouldEqual( @"times" );
        private It should_have_correct_request_uri = () => request.RequestUri.ShouldEqual( @"/fun/times" );
        private It should_have_first_parameter = () => request.Parameters["id"].ShouldEqual( "101" );
        private It should_have_second_parameter = () => request.Parameters["rev"].ShouldEqual( "234" );
        private It should_have_correct_scheme = () => request.Scheme.ShouldEqual( "HTTP" );
        private It should_have_version = () => request.Version.ShouldEqual( "1.1" );
        private It should_have_host_header = () => request.Headers["Host"].ShouldEqual( "localhost:8080" );
        private It should_be_very_fast = () => Timer.ElapsedMilliseconds.ShouldBeLessThanOrEqualTo( 1 );
    }

    public class when_parsing_full_uri_get_request
        : with_full_uri_get_request
    {
        public static Request request;
        
        private Because of = () =>
            {
                Timer = Stopwatch.StartNew();
                request = RequestParser.CreateRequest( new ArraySegment<byte>( bytes ) );
                Timer.Stop();
            };

        private It should_have_correct_method = () => request.Method.ShouldEqual( "GET" );
        private It should_have_correct_base_url = () => request.BaseUri.ShouldEqual( @"http://localhost:8080" );
        private It should_have_first_segment = () => request.PathSegments[0].ShouldEqual( @"fun" );
        private It should_have_second_segment = () => request.PathSegments[1].ShouldEqual( @"times" );
        private It should_have_correct_request_uri = () => request.RequestUri.ShouldEqual( @"/fun/times" );
        private It should_have_first_parameter = () => request.Parameters["id"].ShouldEqual( "101" );
        private It should_have_second_parameter = () => request.Parameters["rev"].ShouldEqual( "234" );
        private It should_have_correct_scheme = () => request.Scheme.ShouldEqual( "HTTP" );
        private It should_have_version = () => request.Version.ShouldEqual( "1.1" );
        private It should_have_host_header = () => request.Headers["Content-Type"].ShouldEqual( "text/plain" );
        private It should_be_very_fast = () => Timer.ElapsedMilliseconds.ShouldBeLessThanOrEqualTo( 1 );
    }

    public class when_parsing_relative_put_request
        : with_relative_put_request
    {
        public static Request request;
        public static string requestBody;

        private Because of = () =>
            {
                Timer = Stopwatch.StartNew();
                request = RequestParser.CreateRequest( new ArraySegment<byte>( bytes ) );
                var arraySegment = request.RequestChunks.Dequeue();
                var bodyBytes = new byte[arraySegment.Count];
                Buffer.BlockCopy( arraySegment.Array, arraySegment.Offset, bodyBytes, 0, arraySegment.Count );
                requestBody = Encoding.UTF8.GetString( bodyBytes );
                Timer.Stop();
            };

        private It should_have_correct_method = () => request.Method.ShouldEqual( "PUT" );
        private It should_have_correct_base_url = () => request.BaseUri.ShouldEqual( @"HTTP://localhost:8080" );
        private It should_have_first_segment = () => request.PathSegments[0].ShouldEqual( @"fun" );
        private It should_have_second_segment = () => request.PathSegments[1].ShouldEqual( @"times" );
        private It should_have_third_segment = () => request.PathSegments[2].ShouldEqual( @"100" );
        private It should_have_correct_request_uri = () => request.RequestUri.ShouldEqual( @"/fun/times/100" );
        private It should_have_correct_scheme = () => request.Scheme.ShouldEqual( "HTTP" );
        private It should_have_version = () => request.Version.ShouldEqual( "1.1" );
        private It should_have_content_type = () => request.Headers["Content-Type"].ShouldEqual( "text/plain" );
        private It should_have_content_length = () => request.Headers["Content-Length"].ShouldEqual( "11" );
        private It should_have_request_body = () => requestBody.ShouldEqual( "This is fun" );

        private It should_be_very_fast = () => Timer.ElapsedMilliseconds.ShouldBeLessThanOrEqualTo( 1 );
    }
}
