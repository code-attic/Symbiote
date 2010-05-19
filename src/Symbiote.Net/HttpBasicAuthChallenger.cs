using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Symbiote.Core.Extensions;

namespace Symbiote.Net
{
    public abstract class AuthRequiredResponse
    {
        public const string HTTP_HEADER_TAG = @"HTTP/{0} 401 Authorization Required";
        public const string DEFAULT_HTTP_VERSION = "1.0";
        public const string SERVER_TAG = @"Server: HTTPd/{0}";
        public const string WWW_AUTH_TAG = @"WWW-Authenticate: {0}";
        public const string CONTENT_TYPE = @"Content-Type: text/html";
        public const string CONTENT_LENGTH = @"Content-Length: 311";

        public const string HTML_AUTH_REQ_RESPONSE = @"
<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN""
 ""http://www.w3.org/TR/1999/REC-html401-19991224/loose.dtd"">
<HTML>
  <HEAD>
    <TITLE>Error</TITLE>
    <META HTTP-EQUIV=""Content-Type"" CONTENT=""text/html; charset=ISO-8859-1"">
  </HEAD>
  <BODY><H1>401 Unauthorized.</H1></BODY>
</HTML>";

        public string HttpVersion { get; set; }

        public abstract string BuildAuthenticateString();

        public override string ToString()
        {
            return string.Join(
                @"\r\n",
                HTTP_HEADER_TAG.AsFormat(HttpVersion ?? DEFAULT_HTTP_VERSION),
                SERVER_TAG.AsFormat(HttpVersion ?? DEFAULT_HTTP_VERSION), 
                BuildAuthenticateString(),
                CONTENT_TYPE,
                CONTENT_LENGTH,
                HTML_AUTH_REQ_RESPONSE
                );
        }
    }

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

    public class HttpBasicAuthChallenger : IHttpAuthChallenge
    {
        protected IHttpServerConfiguration Configuration { get; set; }
        protected readonly byte[] challenge;

        public void ChallengeClient(Stream stream)
        {
            stream.BeginWrite(challenge, 0, challenge.Length, FinishedChallenge, stream);
        }

        private void FinishedChallenge(IAsyncResult ar)
        {
            var stream = ar.AsyncState as Stream;
            stream.EndWrite(ar);
        }

        public HttpBasicAuthChallenger(IHttpServerConfiguration configuration)
        {
            Configuration = configuration;
            var challengeBody = new BasicAuthRequiredResponse(configuration.BaseUrl).ToString();
            challenge = Encoding.UTF8.GetBytes(challengeBody);
        }
    }

    public class HttpBasicAuthProvider : IHttpAuthValidator
    {
        public bool ValidateAuthenticationAttempt(Stream stream)
        {
            return false;
        }
    }

    public interface IHttpAuthChallenge
    {
        void ChallengeClient(Stream stream);
    }

    public interface IHttpAuthValidator
    {
        bool ValidateAuthenticationAttempt(Stream stream);
    }
}
