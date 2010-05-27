using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Symbiote.Net
{
    public class HttpBasicAuthChallenger 
        : IHttpAuthChallenger,
          IObserver<string>
    {
        protected IHttpServerConfiguration Configuration { get; set; }
        protected readonly byte[] challenge;
        protected IAuthenticationValidator Validator { get; set; }
        protected HttpStreamReader reader;
        protected Action<bool, HttpContext> callback;
        protected HttpRequest request;
        protected IHttpClient httpClient;

        public void ChallengeClient(IHttpClient client, Action<bool, HttpContext> onCompletion)
        {
            httpClient = client;
            reader = new HttpStreamReader(client.Stream);
            reader.Subscribe(this);
            callback = onCompletion;
            reader = HttpStreamReader.StartNew(httpClient.Stream, this);
            httpClient.Stream.BeginWrite(challenge, 0, challenge.Length, FinishedChallenge, httpClient.Stream);
        }

        private void FinishedChallenge(IAsyncResult ar)
        {
            var stream = ar.AsyncState as Stream;
            stream.EndWrite(ar);
            stream.Flush();
        }

        private void CompleteAuthentication(bool success)
        {
            reader.Stop();
            reader.Dispose();
            callback(success, new HttpContext(httpClient, request));
        }

        public void OnNext(string value)
        {
            request = new HttpRequest(value);
            var decoded =
                Encoding.UTF8.GetString(Convert.FromBase64String(request.Headers[HttpRequestHeader.Authorization]));
            var credentials = decoded.Split(':');
            CompleteAuthentication(Validator.ValidateCredentials(credentials[0], credentials[1]));
        }

        public void OnError(Exception error)
        {
            reader.Dispose();
            callback(false, null);
        }

        public void OnCompleted()
        {

        }

        public HttpBasicAuthChallenger(IHttpServerConfiguration configuration, IAuthenticationValidator authenticationValidator)
        {
            Configuration = configuration;
            var challengeBody = new BasicAuthRequiredResponse(configuration.BaseUrl).ToString();
            challenge = Encoding.UTF8.GetBytes(challengeBody);
            Validator = authenticationValidator;
        }
    }
}
