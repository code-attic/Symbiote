using System;

namespace Symbiote.Http.NetAdapter.Socket
{
    public interface IOwinObservable
    {
        Action Setup( IOwinObserver observer );
        Action Setup( Func<ArraySegment<byte>, Action, bool> onNext, Action<Exception> onError, Action complete );
    }
}