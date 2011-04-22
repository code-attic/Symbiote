/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.IO;
using System.Net;
using System.Text;
using Symbiote.Net;

namespace Symbiote.Http.NetAdapter.TcpListener
{
    public class HttpBasicAuthChallenger 
        : IHttpAuthChallenger,
          IObserver<string>
    {
        protected SocketConfiguration Configuration { get; set; }
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

        public HttpBasicAuthChallenger(SocketConfiguration configuration, IAuthenticationValidator authenticationValidator)
        {
            Configuration = configuration;
            var challengeBody = new BasicAuthRequiredResponse("baseUrl").ToString();
            challenge = Encoding.UTF8.GetBytes(challengeBody);
            Validator = authenticationValidator;
        }
    }
}
