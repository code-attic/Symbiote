using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Symbiote.Http.Impl.Adapter
{
    public class ResponseEncoder
    {
        static Dictionary<Type, Action<object, Stream>> Writers = new Dictionary<Type, Action<object, Stream>>()
        {
            { typeof( byte[] ), ( o, s ) => WriteByteArray( o as byte[], s ) },
            { typeof( string ), ( o, s ) => WriteString( o as string, s ) },
            { typeof( ArraySegment<byte> ), ( o, s ) => WriteByteSegment( ( ArraySegment<byte> ) o, s ) },
            { typeof( FileInfo ), ( o, s ) => WriteFile( o as FileInfo, s ) },
            { typeof( Action<Stream> ), ( o, s ) => WriteStream( o as Action<Stream>, s ) },
        };

        public static void Write( object item, Stream stream )
        {
            Writers[item.GetType()]( item, stream );
        }

        static void WriteByteArray( byte[] buffer, Stream stream )
        {
            stream.Write( buffer, 0, buffer.Length );
        }

        static void WriteByteSegment( ArraySegment<byte> buffer, Stream stream )
        {
            WriteByteArray( buffer.Array, stream );
        }

        static void WriteFile( FileInfo info, Stream stream )
        {
            using(var fileStream = info.OpenRead())
            {
                var buffer = new byte[info.Length];
                var read = fileStream.Read( buffer, 0, buffer.Length );
                stream.Write( buffer, 0, read );
            }
        }

        static void WriteStream(Action<Stream> writer, Stream stream)
        {
            writer( stream );
        }

        static void WriteString( string content, Stream stream )
        {
            var buffer = Encoding.UTF8.GetBytes( content );
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
