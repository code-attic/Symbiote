using System;
using System.Diagnostics;
using System.Text;
using Machine.Specifications;
using Symbiote.Http.Owin.Impl;

namespace Http.Tests
{
    public class when_parsing_relative_put_request
        : with_relative_put_request
    {
        public static Request request;
        public static string requestBody;

        private Because of = () =>
                                 {
                                     Timer = Stopwatch.StartNew();
                                     request = new Request( x => requestBody = Encoding.UTF8.GetString( x.Array, x.Offset, x.Count ) );
                                     RequestParser.PopulateRequest( request, new ArraySegment<byte>( bytes ) );
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