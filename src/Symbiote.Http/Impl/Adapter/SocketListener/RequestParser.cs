using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Http.Owin;

namespace Symbiote.Http.Impl.Adapter.SocketListener 
{
    public class RequestParser
        : HttpConstants
    {
        public static Request CreateRequest( ArraySegment<byte> arraySegment )
        {
            var request = new Request();
            request.OnData( arraySegment );
            return request;
        }

        public static void PopulateRequest( Request request, ArraySegment<byte> arraySegment )
        {
            var last = ParseLine( request, arraySegment );
            last = ParseHeaders( request, arraySegment, last ) + 2;
            if( last + arraySegment.Offset < arraySegment.Count )
            {
                var bodySegment = new ArraySegment<byte>( arraySegment.Array, last, arraySegment.Count - last );
                request.OnData( bodySegment );
            }
        }

        public static int ParseHeaders( Request request, ArraySegment<byte> buffer, int start )
        {
            var index = start-1;
            var comparer = new HeaderKeyEqualityComparer();
            request.Headers = new Dictionary<string, string>( comparer );
            while ( ++index + 4 < buffer.Count && buffer.Array[index] != CR )
            {
                var headerName = ParseValue( buffer, COLON, index, ref index );
                var value = ParseValue( buffer, CR, index, ref index ).Trim();
                request.Headers.Add( headerName, value );
            }

            var host = "";
            if( request.Headers.TryGetValue( "Host", out host ) )
                request.BaseUri = string.Format( "{0}://{1}", request.Scheme, host );

            return index++;
        }

        public static int ParseLine( Request request, ArraySegment<byte> buffer )
        {
            var lastRead = 0;
            lastRead = GetMethod( request, buffer, lastRead );
            lastRead = GetUri( request, buffer, lastRead );
            lastRead = GetVersion( request, buffer, lastRead );
            return lastRead;
        }

        public static string ParseValue( ArraySegment<byte> buffer, byte stopCharacter, int start, ref int index )
        {
            var length = 1;
            while ( ++index < buffer.Count && buffer.Array[ index + buffer.Offset ] != stopCharacter )
            {
                length++;
            }
            index++;
            return Encoding.UTF8.GetString( buffer.Array, start, length );
        }

        public static string ParseBaseUri( ArraySegment<byte> buffer, int start, ref int index )
        {
            var length = 1;
            var count = 0;
            while 
            ( 
                ++index < buffer.Count && 
                ( ( count += buffer.Array[ index + buffer.Offset ] == SLASH ? 1 : 0 ) < 3 )
            )
            {
                length++;
            }
            return Encoding.UTF8.GetString( buffer.Array, start, length );
        }

        public static int GetMethod( Request request, ArraySegment<byte> buffer, int start )
        {
            var index = start;
            request.Method = ParseValue( buffer, SPACE, start, ref index );
            return index;
        }

        public static int GetUri( Request request, ArraySegment<byte> buffer, int start )
        {
            return buffer.Array[start + buffer.Offset] == SLASH
                       ? GetRelativeUri( request, buffer, start )
                       : GetAbsoluteUri( request, buffer, start );
        }

        public static int GetRelativeUri( Request request, ArraySegment<byte> buffer, int start )
        {
            var index = start;
            var pathAndQuery = ParseValue( buffer, SPACE, start, ref index );
            ParsePathAndQuery( request, pathAndQuery );
            return index;
        }

        public static int GetAbsoluteUri( Request request, ArraySegment<byte> buffer, int start )
        {
            var index = start;
            request.BaseUri = ParseBaseUri( buffer, start, ref index );
            var pathAndQuery = ParseValue( buffer, SPACE, index, ref index );
            ParsePathAndQuery( request, pathAndQuery );

            return index;
        }

        public static void ParsePathAndQuery( Request request, string pathAndQuery )
        {
            var pathLength = 1;
            var index = 0;
            while( ++index < pathAndQuery.Length && pathAndQuery[index] != QMARK )
            {
                pathLength++;
            }
            request.RequestUri = pathAndQuery.Substring( 0, pathLength );
            request.PathSegments = GetSegments( request ).ToList();
            index++;
            var queryString = index > pathAndQuery.Length ? "" : pathAndQuery.Substring( index );
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

        public static IEnumerable<string> GetSegments( Request request )
        {
            string requestUri = request.RequestUri;
            if ( requestUri.Length < 2 )
                yield break;

            var index = 1;
            var length = 0;
            var start = index;

            while ( index < requestUri.Length )
            {
                while ( index < requestUri.Length && requestUri[index] != SLASH )
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

        public static int GetVersion( Request request, ArraySegment<byte> buffer, int start )
        {
            var index = start;
            request.Scheme = ParseValue( buffer, SLASH, start, ref index );
            request.Version = ParseValue( buffer, CR, index, ref index );
            return index + 1;
        }

        public static IDictionary<string, object> GetOwinItems( IRequest request )
        {
            var requestBody = new Action<Action<ArraySegment<byte>>, Action<Exception>>( request.ReadNext );

            return new Dictionary<string, object>()
                {
                    { Owin.ItemKeys.REQUEST_METHOD, request.Method },   
                    { Owin.ItemKeys.REQUEST_URI, request.RequestUri },
                    { Owin.ItemKeys.REQUEST_HEADERS, request.Headers },
                    { Owin.ItemKeys.BASE_URI, request.BaseUri },
                    { Owin.ItemKeys.SERVER_NAME, request.Server },
                    { Owin.ItemKeys.URI_SCHEME, request.Scheme },
                    { Owin.ItemKeys.REMOTE_ENDPOINT, request.ClientEndpoint },
                    { Owin.ItemKeys.VERSION, request.Version },
                    { Owin.ItemKeys.REQUEST, request },
                    { Owin.ItemKeys.REQUEST_BODY, requestBody },
                };
        }
    }
}
