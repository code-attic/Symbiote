using System;

namespace Symbiote.Http.NetAdapter.Socket
{
    public interface IOwinObserver
    {
        bool OnNext( ArraySegment<byte> segment, Action continuation );
        void OnError( Exception exception );
        void OnComplete();
    }
}