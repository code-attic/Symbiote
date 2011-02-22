using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Http
{
    public delegate void OwinApplication(IDictionary<string, object> request,
                        OwinResponse response,
                        Action<Exception> exception);

    public delegate void OwinResponse(string status, IDictionary<string, string> headers, IEnumerable<object> body);
}
