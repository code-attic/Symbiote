using System;
using System.Collections.Generic;

namespace Symbiote.Http
{
    public delegate void OwinApplication(
                    IDictionary<string, object> request,
                    OwinResponse response,
                    Action<Exception> exception);

    public delegate Action OwinBody(
                    Func<ArraySegment<byte>, Action, bool> onNext,
                    Action<Exception> onError,
                    Action onComplete );

    public delegate void OwinResponse(
                    string status,
                    IDictionary<string, string> headers,
                    OwinBody body);
}