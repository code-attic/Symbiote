using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Symbiote.Riak.Impl.ProtoBuf
{
    public static class ByteExtensions
    {
        private static readonly int BUFFER_LENGTH = 8 * 1024;

        public static string FromBytes( this byte[] bytes )
        {
            return Encoding.UTF8.GetString( bytes );
        }

        public static byte[] ToBytes( this string value )
        {
            return value == null ? null : Encoding.UTF8.GetBytes( value );
        }

        public static byte[] ToBytes( this NetworkStream stream )
        {
            var buffer = new byte[BUFFER_LENGTH];
            using(var memoryStream = new MemoryStream())
            {
                while(stream.DataAvailable)
                {
                    var read = stream.Read( buffer, 0, buffer.Length );
                    memoryStream.Write( buffer, 0, read );
                }
                return memoryStream.ToArray();
            }
        }
    }
}