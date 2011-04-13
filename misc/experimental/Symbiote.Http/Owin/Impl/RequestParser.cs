using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Http.Owin.Impl
{
    public class RequestParser
        : HttpConstants
    {
        public static void PopulateRequest( Request request, ConsumableSegment<byte> arraySegment )
        {
            while( arraySegment.Count > 0 )
                request.Parse( request, arraySegment );
        }

        public static void PassBodySegmentOn( Request request, ConsumableSegment<byte> buffer )
        {
            if( request.OnBody != null )
                request.OnBody( buffer );
            buffer.Offset += buffer.Count;
        }

        public static void ParseHeaders( Request request, ConsumableSegment<byte> buffer )
        {
            var comparer = new HeaderKeyEqualityComparer();
            request.Headers = new Dictionary<string, string>( comparer );
            var length = buffer.Length;
            
            while ( buffer.Next() && buffer.Offset + 4 < length && ( buffer.Array[buffer.Offset] != CR && buffer.Array[buffer.Offset + 2] != CR ) )
            {
                var headerName = ParseValue( buffer, COLON );
                var value = ParseValue( buffer, CR ).Trim();
                request.Headers.Add( headerName, value );
            }

            var host = "";
            if( request.Headers.TryGetValue( "Host", out host ) )
                request.BaseUri = string.Format( "{0}://{1}", request.Scheme, host );
            
            while ( buffer.Next() && ( buffer.Array[buffer.Offset] == CR || buffer.Array[buffer.Offset] == LF ) )
            {
            }

            request.HeadersComplete = true;
            request.Parse = PassBodySegmentOn;
        }

        public static void ParseLine( Request request, ConsumableSegment<byte> buffer )
        {
            GetMethod( request, buffer );
            GetUri( request, buffer );
            GetVersion( request, buffer );
            request.Parse = ParseHeaders;
        }

        public static string ParseValue( ConsumableSegment<byte> buffer, byte stopCharacter )
        {
            var length = 1;
            var start = buffer.Offset;
            while ( buffer.Next() && buffer.Array[ buffer.Offset ] != stopCharacter )
            {
                length++;
            }
            buffer.Offset++;
            return Encoding.UTF8.GetString( buffer.Array, start, length );
        }

        public static string ParseBaseUri( ConsumableSegment<byte> buffer )
        {
            var length = 1;
            var count = 0;
            var start = buffer.Offset;
            while( buffer.Next() && 
                ( ( count += buffer.Array[ buffer.Offset ] == SLASH ? 1 : 0 ) < 3 ) )
            {
                length++;
            }
            return Encoding.UTF8.GetString( buffer.Array, start, length );
        }

        public static void GetMethod( Request request, ConsumableSegment<byte> buffer )
        {
            request.Method = ParseValue( buffer, SPACE );
        }

        public static void GetUri( Request request, ConsumableSegment<byte> buffer )
        {
            if( buffer.Array[ buffer.Offset ] == SLASH )
                GetRelativeUri( request, buffer );
            else
                GetAbsoluteUri( request, buffer );
        }

        public static void GetRelativeUri( Request request, ConsumableSegment<byte> buffer )
        {
            var pathAndQuery = ParseValue( buffer, SPACE );
            ParsePathAndQuery( request, pathAndQuery );
        }

        public static void GetAbsoluteUri( Request request, ConsumableSegment<byte> buffer )
        {
            request.BaseUri = ParseBaseUri( buffer );
            var pathAndQuery = ParseValue( buffer, SPACE );
            ParsePathAndQuery( request, pathAndQuery );
        }

        public static void ParsePathAndQuery( Request request, string pathAndQuery )
        {
            var pathLength = 1;
            var index = 0;
            var length = pathAndQuery.Length;
            while( ++index < length && pathAndQuery[index] != QMARK )
            {
                pathLength++;
            }
            request.RequestUri = pathAndQuery.Substring( 0, pathLength );
            request.PathSegments = GetSegments( request ).ToList();
            index++;
            var queryString = index > length ? "" : pathAndQuery.Substring( index );
            request.Parameters = GetParameters( queryString );
        }

        private static IDictionary<string, string> GetParameters( string query )
        {
            if( string.IsNullOrEmpty( query ))
                return new Dictionary<string, string>();

            var index = 0;
            var length = 0;
            var start = 0;
            var parameters = new Dictionary<string, string>();
            var queryLength = query.Length;
            while( index < queryLength )
            {
                while ( index < queryLength && query[index] != '=' )
                {
                    length++;
                    index++;
                }
                
                if( index >= queryLength)
                    break;

                var key = query.Substring( start, length );
                length = 0;
                index++;
                start = index;

                while ( index < queryLength && query[index] != '&' && query[index] != CR )
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

        public static IEnumerable<string> GetSegments( Request request )
        {
            string requestUri = request.RequestUri;
            var uriLength = requestUri.Length;
            if ( uriLength < 2 )
                yield break;

            var index = 1;
            var length = 0;
            var start = index;

            while ( index < uriLength )
            {
                while ( index < uriLength && requestUri[index] != SLASH )
                {
                    length++;
                    index++;
                }

                yield return requestUri.Substring( start, length );

                index++;
                start = index;
                length = 0;
            }
        }

        public static void GetVersion( Request request, ConsumableSegment<byte> buffer )
        {
            request.Scheme = ParseValue( buffer, SLASH );
            request.Version = ParseValue( buffer, CR );
        }
    }
}