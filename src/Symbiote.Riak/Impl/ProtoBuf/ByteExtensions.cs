using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Symbiote.Riak.Impl.ProtoBuf
{
    public static class ByteExtensions
    {
        private static readonly int BUFFER_SIZE = 4 * 1024;

        public static string FromBytes( this byte[] bytes )
        {
            return Encoding.UTF8.GetString( bytes );
        }

        public static byte[] ToBytes( this string value )
        {
            return Encoding.UTF8.GetBytes( value );
        }

        public static byte[] ToBytes( this Stream stream )
        {
            var buffer = new byte[BUFFER_SIZE];
            using (var memoryStream = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    // exclude the response termination byte
                    var count = buffer[read - 1] == -1
                                    ? read - 1
                                    : read;
                    memoryStream.Write( buffer, 0, count );
                } while ( read > 0 );
                return memoryStream.ToArray();
            }
        }
    }
}