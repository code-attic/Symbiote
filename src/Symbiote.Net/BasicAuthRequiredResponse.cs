using Symbiote.Core.Extensions;

namespace Symbiote.Net
{
    public class BasicAuthRequiredResponse : AuthRequiredResponse
    {
        protected string Realm { get; set; }

        public override string BuildAuthenticateString()
        {
            return @"Basic realm=""{0}""".AsFormat(Realm);
        }

        public BasicAuthRequiredResponse(string realm)
        {
            Realm = realm;
        }
    }
}