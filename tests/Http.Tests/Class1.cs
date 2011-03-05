using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Http.Impl.Adapter;
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

    public abstract class with_full_uri_get_request
        : with_timer
    {
        public static string request_body;
        public static byte[] bytes;

        private Establish context = () =>
            {
                request_body =
@"GET http://localhost:8080/fun/times?id=1&rev=2 HTTP/1.1
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
                request = new Request();
                var parser = new RequestLineParser();
                parser.ParseLine( request, new ArraySegment<byte>( bytes ) );
                Timer.Stop();
            };
        
        private It should_have_correct_method = () => request.Method.ShouldEqual( "GET" );
        private It should_have_correct_url = () => request.FullUri.ToString().ShouldEqual( @"http://localhost:8080/fun/times?id=1&rev=2" );
        private It should_have_correct_request_uri = () => request.RequestUri.ShouldEqual( @"/fun/times" );
        private It should_have_first_parameter = () => request.Parameters["id"].ShouldEqual( "101" );
        private It should_have_second_parameter = () => request.Parameters["rev"].ShouldEqual( "234" );
        private It should_have_correct_scheme = () => request.Scheme.ShouldEqual( "HTTP" );
        private It should_have_version = () => request.Version.ShouldEqual( "1.1" );
        private It should_be_very_fast = () => Timer.ElapsedMilliseconds.ShouldBeLessThanOrEqualTo( 1 );
    }

    public class when_parsing_full_uri_get_request
        : with_full_uri_get_request
    {
        public static Request request;
        
        private Because of = () =>
            {
                Timer = Stopwatch.StartNew();
                request = new Request();
                var parser = new RequestLineParser();
                parser.ParseLine( request, new ArraySegment<byte>( bytes ) );
                Timer.Stop();
            };

        private It should_have_correct_method = () => request.Method.ShouldEqual( "GET" );
        private It should_have_correct_url = () => request.FullUri.ToString().ShouldEqual( @"http://localhost:8080/fun/times?id=1&rev=2" );
        private It should_have_correct_request_uri = () => request.RequestUri.ShouldEqual( @"/fun/times" );
        private It should_have_correct_scheme = () => request.Scheme.ShouldEqual( "HTTP" );
        private It should_have_version = () => request.Version.ShouldEqual( "1.1" );
        private It should_be_very_fast = () => Timer.ElapsedMilliseconds.ShouldBeLessThanOrEqualTo( 1 );
    }


    public class HttpConstants
    {
        protected const byte CR = 0x0d;
		protected const byte LF = 0x0a;
		protected const byte DOT = 0x2e;
		protected const byte SPACE = 0x20;
		protected const byte SEMI = 0x3b;
		protected const byte COLON = 0x3a;
		protected const byte HASH = 0x23;
		protected const byte QMARK = 0x3f;
		protected const byte SLASH = 0x2f;
		protected const byte DASH = 0x2d;
		protected const byte NULL = 0x00;
        protected readonly byte[] LINE_TERMINATOR = new [] { CR, LF };
    }

    public class RequestLineParser
        : HttpConstants
    {
        public Request Request { get; set; }
        public ArraySegment<byte> Buffer { get; set; }
        public bool HasMethod { get; set; }
        public bool HasUri { get; set; }
        public bool HasVersion { get; set; }

        public int ParseLine( Request request, ArraySegment<byte> bytes )
        {
            var lastRead = 0;
            Request = request;
            Buffer = bytes;
            lastRead = GetMethod( lastRead );
            lastRead = GetUri( lastRead );
            lastRead = GetVersion( lastRead );
            return lastRead;
        }

        public string ProcessValue( byte stopCharacter, int start, ref int index )
        {
            var length = 1;
            while ( ++index < Buffer.Count && Buffer.Array[ index + Buffer.Offset ] != stopCharacter )
            {
                length++;
            }
            index++;
            return Encoding.UTF8.GetString( Buffer.Array, start, length );
        }

        public int GetMethod( int start )
        {
            var index = start;
            Request.Method = ProcessValue( SPACE, start, ref index );
            return index;
        }

        public int GetUri( int start )
        {
            return Buffer.Array[start + Buffer.Offset] == SLASH
                       ? GetRelativeUri( start )
                       : GetAbsoluteUri( start );
        }

        public int GetRelativeUri( int start )
        {
            var index = start;
            Request.RequestUri = ProcessValue( QMARK, start, ref index );

            var queryString = ProcessValue( SPACE, index, ref index );
            Request.Parameters = GetParameters( queryString );

            return index;
        }

        public int GetAbsoluteUri( int start )
        {
            return start;
        }

        private IDictionary<string, string> GetParameters( string query )
        {
            if( string.IsNullOrEmpty( query ))
                return new Dictionary<string, string>();

            var index = 0;
            var length = 0;
            var start = 0;
            var parameters = new Dictionary<string, string>();
            while( index < query.Length )
            {
                while ( index < query.Length && query[index] != '=' )
                {
                    length++;
                    index++;
                }
                
                if( index >= query.Length)
                    break;

                var key = query.Substring( start, length );
                length = 0;
                index++;
                start = index;

                while ( index < query.Length && query[index] != '&' && query[index] != CR )
                {   
                    length++;
                    index++;
                }

                var value = query.Substring( start, length );
                length = 0;
                index++;
                start = index;

                parameters.Add( key, value );
            }

            return parameters;
        }

        public int GetVersion( int start )
        {
            var index = start;
            Request.Scheme = ProcessValue( SLASH, start, ref index );
            Request.Version = ProcessValue( CR, index, ref index );
            return index + 1;
        }
    }
}
