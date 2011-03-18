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

namespace Symbiote.Http.NetAdapter.TcpListener
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