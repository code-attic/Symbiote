using System;
using System.Diagnostics;
using Machine.Specifications;
using Symbiote.Http.Owin;
using Symbiote.Http.Owin.Impl;

namespace Http.Tests
{
    public class when_parsing_full_uri_get_request
        : with_full_uri_get_request
    {
        public static Request request;
        
        private Because of = () =>
                                 {
                                     Timer = Stopwatch.StartNew();
                                     request = new Request();
                                     RequestParser.PopulateRequest( request, new ArraySegment<byte>( bytes ) );
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
}
