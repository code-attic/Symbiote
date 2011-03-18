using System;
using System.Collections.Generic;

namespace Symbiote.Http
{
    public delegate void OwinApplication(
                    IDictionary<string, object> request,
                    OwinResponse response,
                    Action<Exception> exception);
}