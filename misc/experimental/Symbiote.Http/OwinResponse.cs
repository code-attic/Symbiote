using System.Collections.Generic;

namespace Symbiote.Http
{
    public delegate void OwinResponse(
        string status,
        IDictionary<string, string> headers,
        OwinBody body);
}