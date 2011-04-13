using System;
using Symbiote.Net;

namespace Symbiote.Http.NetAdapter.Socket
{
    public class ResponseWriter
        : IOwinObserver
    {
        public ISocketAdapter Connection { get; set; }
        public Action Close { get; set; }

        public bool OnNext( ArraySegment<byte> segment, Action continuation )
        {
            Connection.Write( segment, continuation );
            return true;
        }

        public void OnError( Exception exception )
        {
            Console.WriteLine( exception.Message );
            Close();
        }

        public void OnComplete()
        {
            Close();
        }

        public ResponseWriter( ISocketAdapter connection, Action close )
        {
            Connection = connection;
            Close = close;
        }
    }
}