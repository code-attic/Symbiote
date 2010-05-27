using System;

namespace Symbiote.Net
{
    public class NoAuthChallenger
        : IHttpAuthChallenger,
          IObserver<string>
    {
        protected HttpStreamReader reader;
        protected Action<bool, HttpContext> callback;
        protected IHttpClient httpClient;
        protected HttpRequest request;

        public void OnNext(string value)
        {
            request = new HttpRequest(value);
            callback(true, new HttpContext(httpClient, request));
        }

        public void OnError(Exception error)
        {
            callback(false, null);
        }

        public void OnCompleted()
        {
            
        }

        public void ChallengeClient(IHttpClient client, Action<bool, HttpContext> onCompletion)
        {
            httpClient = client;
            reader = HttpStreamReader.StartNew(client.Stream, this);
            callback = onCompletion;
        }
    }
}