using System;
using System.Collections.Generic;
using Symbiote.Net;

namespace Symbiote.Http.NetAdapter.Socket
{
    public class RequestReader
        : IOwinObservable
    {
        public ISocketAdapter Connection { get; set; }
        public Func<ArraySegment<byte>, Action, bool> OnNextRead { get; set; }
        public Action<Exception> OnReadError { get; set; }
        public Action OnReadComplete { get; set; }
        public Queue<ArraySegment<byte>> RequestChunks { get; set; }
        public Action<ArraySegment<byte>> OnData { get; set; }
        public Action Close { get; set; }

        public void CacheBytes( ArraySegment<byte> bytes )
        {
            RequestChunks.Enqueue( bytes );
        }

        public void OnBytes( ArraySegment<byte> bytes )
        {
            if( bytes.Count == 0 )
            {
                OnReadComplete();
            }
            else if( !OnNextRead( bytes, ReadNext ) )
            {
                ReadNext();
            }
        }

        public void ReadNext()
        {
            if( RequestChunks.Count > 0 )
            {
                OnBytes( RequestChunks.Dequeue() );
            }
            else
            {
                if( !Connection.Read( OnBytes ) )
                    OnReadComplete();
            }
        }

        public Action Setup( IOwinObserver observer )
        {
            return Setup( observer.OnNext, observer.OnError, observer.OnComplete );
        }

        public Action Setup( Func<ArraySegment<byte>, Action, bool> onNext, Action<Exception> onError, Action complete )
        {
            OnNextRead = onNext;
            OnReadError = onError;
            OnReadComplete = complete;
            return Close;
        }

        public RequestReader( ISocketAdapter connection, Action close )
        {
            RequestChunks = new Queue<ArraySegment<byte>>();
            Connection = connection;
            Close = close;
        }
    }
}